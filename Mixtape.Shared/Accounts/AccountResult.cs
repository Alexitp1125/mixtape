namespace Mixtape.Shared.Accounts
{
    // Result of creating an account or signing in.
    public sealed class AccountResult
    {
        public AccountStatus Status { get; init; }

        // The account, when Status is Ok; null otherwise.
        public AccountInfo? Account { get; init; }
    }
}
