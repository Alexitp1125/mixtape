using Mixtape.Shared.Accounts;

namespace Mixtape.Application.Accounts
{
    public sealed partial class AccountManager : IAccountsService
    {
        Task<AccountResult> IAccountsService.CreateAsync(string name, string pin, CancellationToken cancellationToken) =>
            CreateAsync(name, pin, cancellationToken);

        Task<AccountResult> IAccountsService.SignInAsync(string name, string pin, CancellationToken cancellationToken) =>
            SignInAsync(name, pin, cancellationToken);
    }
}
