using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mixtape.Data;
using Mixtape.Data.Entities;
using Mixtape.Shared;
using Mixtape.Shared.Accounts;

namespace Mixtape.Application.Accounts
{
    // Creates accounts, checks the name and the PIN of who signs in, changes the PIN and deletes accounts.
    public sealed partial class AccountManager
    {
        // Wrong PINs in a row that lock an account, and for how long.
        private const int MaxFailures = 5;
        private static readonly TimeSpan LockTime = TimeSpan.FromMinutes(15);

        // New accounts per hour, counting the whole site.
        private const int MaxNewAccountsPerHour = 30;

        private readonly IDbContextFactory<MixtapeDbContext> dbContextFactory;
        private readonly ILogger<AccountManager> logger;
        private readonly PasswordHasher<User> pinHasher = new();

        public AccountManager(
            IDbContextFactory<MixtapeDbContext> dbContextFactory,
            ILogger<AccountManager> logger)
        {
            this.dbContextFactory = dbContextFactory;
            this.logger = logger;
        }

        // Creates an account. The PIN is stored hashed.
        public async Task<AccountResult> CreateAsync(string name, string pin, CancellationToken cancellationToken = default)
        {
            try
            {
                string cleanName = name.Trim();

                if (cleanName.Length is < 3 or > 30)
                {
                    return Failed(AccountStatus.NameInvalid);
                }

                if (!IsPin(pin))
                {
                    return Failed(AccountStatus.PinInvalid);
                }

                string nameKey = cleanName.ToLowerInvariant();
                DateTime now = DateTime.UtcNow;
                DateTime oneHourAgo = now.AddHours(-1);

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                if (await dbContext.Users.AnyAsync(x => x.NameKey == nameKey, cancellationToken))
                {
                    return Failed(AccountStatus.NameTaken);
                }

                if (await dbContext.Users.CountAsync(x => x.CreatedOn > oneHourAgo, cancellationToken) >= MaxNewAccountsPerHour)
                {
                    return Failed(AccountStatus.TooManyAccounts);
                }

                User user = new() { Name = cleanName, NameKey = nameKey, CreatedOn = now };
                user.PinHash = pinHasher.HashPassword(user, pin);

                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(user);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error creating the account.", logger);
            }
        }

        // Checks the name and the PIN. Counts the wrong PINs and locks the account after too many.
        public async Task<AccountResult> SignInAsync(string name, string pin, CancellationToken cancellationToken = default)
        {
            try
            {
                string nameKey = name.Trim().ToLowerInvariant();
                DateTime now = DateTime.UtcNow;

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.NameKey == nameKey, cancellationToken);

                if (user is null)
                {
                    return Failed(AccountStatus.WrongNameOrPin);
                }

                AccountStatus status = CheckPin(user, pin, now);
                await dbContext.SaveChangesAsync(cancellationToken);

                // Signing in never says which of the two was wrong.
                return status switch
                {
                    AccountStatus.Ok => Succeeded(user),
                    AccountStatus.WrongPin => Failed(AccountStatus.WrongNameOrPin),
                    _ => Failed(status),
                };
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error signing in.", logger);
            }
        }

        // Changes the PIN of an account. Asks for the current one, and its wrong tries count like the ones of signing in.
        public async Task<AccountResult> ChangePinAsync(Guid userKey, string currentPin, string newPin, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!IsPin(newPin))
                {
                    return Failed(AccountStatus.PinInvalid);
                }

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Key == userKey, cancellationToken);

                if (user is null)
                {
                    return Failed(AccountStatus.NotFound);
                }

                AccountStatus status = CheckPin(user, currentPin, DateTime.UtcNow);

                if (status == AccountStatus.Ok)
                {
                    user.PinHash = pinHasher.HashPassword(user, newPin);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                return status == AccountStatus.Ok ? Succeeded(user) : Failed(status);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error changing the PIN.", logger);
            }
        }

        // Deletes an account with its mixtapes. Asks for the PIN. The database deletes the songs with their mixtape.
        public async Task<AccountResult> DeleteAsync(Guid userKey, string pin, CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                User? user = await dbContext.Users.FirstOrDefaultAsync(x => x.Key == userKey, cancellationToken);

                if (user is null)
                {
                    return Failed(AccountStatus.NotFound);
                }

                AccountStatus status = CheckPin(user, pin, DateTime.UtcNow);

                if (status == AccountStatus.Ok)
                {
                    dbContext.Artists.RemoveRange(await dbContext.Artists.Where(x => x.UserKey == userKey).ToListAsync(cancellationToken));
                    dbContext.Users.Remove(user);
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                return status == AccountStatus.Ok ? Succeeded(user) : Failed(status);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error deleting the account.", logger);
            }
        }

        // Checks the PIN of an account and keeps its count of wrong PINs: too many in a row lock it for a while.
        // It changes the account; whoever calls saves the changes.
        private AccountStatus CheckPin(User user, string pin, DateTime now)
        {
            if (user.LockedUntil > now)
            {
                return AccountStatus.Locked;
            }

            if (pinHasher.VerifyHashedPassword(user, user.PinHash, pin) == PasswordVerificationResult.Failed)
            {
                bool locks = user.FailedCount + 1 >= MaxFailures;

                user.FailedCount = locks ? 0 : user.FailedCount + 1;
                user.LockedUntil = locks ? now + LockTime : null;

                return locks ? AccountStatus.Locked : AccountStatus.WrongPin;
            }

            user.FailedCount = 0;
            user.LockedUntil = null;

            return AccountStatus.Ok;
        }

        // A PIN is exactly 4 digits.
        private static bool IsPin(string pin) =>
            pin.Length == 4 && pin.All(char.IsAsciiDigit);

        private static AccountResult Failed(AccountStatus status) =>
            new() { Status = status };

        private static AccountResult Succeeded(User user) =>
            new() { Status = AccountStatus.Ok, Account = new AccountInfo { Key = user.Key, Name = user.Name } };
    }
}
