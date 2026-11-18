namespace Mixtape.Shared.Accounts
{
    // The account of who signed in.
    public sealed class AccountInfo
    {
        public Guid Key { get; init; }

        public string Name { get; init; } = null!;
    }
}
