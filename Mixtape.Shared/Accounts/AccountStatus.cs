namespace Mixtape.Shared.Accounts
{
    // How creating an account or signing in ended.
    public enum AccountStatus
    {
        Ok,

        // The name doesn't have 3 to 30 characters.
        NameInvalid,

        // The PIN isn't 4 digits.
        PinInvalid,

        // Another account already has that name.
        NameTaken,

        WrongNameOrPin,

        // Too many wrong PINs in a row: the account is locked for a while.
        Locked,

        // Too many accounts were created in the last hour.
        TooManyAccounts,
    }
}
