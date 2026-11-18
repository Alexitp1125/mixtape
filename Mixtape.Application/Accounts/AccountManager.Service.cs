using Mixtape.Shared.Accounts;

namespace Mixtape.Application.Accounts
{
    public sealed partial class AccountManager : IAccountsService
    {
        Task<AccountResult> IAccountsService.CreateAsync(string name, string pin, CancellationToken cancellationToken) =>
            CreateAsync(name, pin, cancellationToken);

        Task<AccountResult> IAccountsService.SignInAsync(string name, string pin, CancellationToken cancellationToken) =>
            SignInAsync(name, pin, cancellationToken);

        Task<AccountResult> IAccountsService.ChangePinAsync(Guid userKey, string currentPin, string newPin, CancellationToken cancellationToken) =>
            ChangePinAsync(userKey, currentPin, newPin, cancellationToken);

        Task<AccountResult> IAccountsService.DeleteAsync(Guid userKey, string pin, CancellationToken cancellationToken) =>
            DeleteAsync(userKey, pin, cancellationToken);
    }
}
