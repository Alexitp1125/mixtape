using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mixtape.Data;
using Mixtape.Data.Entities;
using Mixtape.Shared;
using Mixtape.Shared.Accounts;

namespace Mixtape.Application.Accounts
{
    // Creates accounts and checks the name and the PIN of who signs in.
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

                if (user.LockedUntil > now)
                {
                    return Failed(AccountStatus.Locked);
                }

                if (pinHasher.VerifyHashedPassword(user, user.PinHash, pin) == PasswordVerificationResult.Failed)
                {
                    bool locks = user.FailedCount + 1 >= MaxFailures;

                    user.FailedCount = locks ? 0 : user.FailedCount + 1;
                    user.LockedUntil = locks ? now + LockTime : null;
                    await dbContext.SaveChangesAsync(cancellationToken);

                    return Failed(locks ? AccountStatus.Locked : AccountStatus.WrongNameOrPin);
                }

                user.FailedCount = 0;
                user.LockedUntil = null;
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(user);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error signing in.", logger);
            }
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
