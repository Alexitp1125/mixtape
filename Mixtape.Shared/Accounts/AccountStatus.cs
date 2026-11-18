namespace Mixtape.Shared.Accounts
{
    // How creating an account, signing in or changing an account ended.
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

        // The PIN isn't the one of the account (to change the PIN or to delete the account).
        WrongPin,

        // The account doesn't exist any more.
        NotFound,

        // Too many wrong PINs in a row: the account is locked for a while.
        Locked,

        // Too many accounts were created in the last hour.
        TooManyAccounts,
    }
}
