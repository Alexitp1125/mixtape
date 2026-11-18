namespace Mixtape.Shared.Accounts
{
    public interface IAccountsService
    {
        // Creates an account with a name (3 to 30 characters, not taken) and a PIN (4 digits).
        Task<AccountResult> CreateAsync(string name, string pin, CancellationToken cancellationToken = default);

        // Checks the name and the PIN of an account.
        Task<AccountResult> SignInAsync(string name, string pin, CancellationToken cancellationToken = default);

        // Changes the PIN of an account. Asks for the current one.
        Task<AccountResult> ChangePinAsync(Guid userKey, string currentPin, string newPin, CancellationToken cancellationToken = default);

        // Deletes an account with its mixtapes and their songs. Asks for the PIN.
        Task<AccountResult> DeleteAsync(Guid userKey, string pin, CancellationToken cancellationToken = default);
    }
}
