using System.Security.Claims;

namespace Mixtape.Accounts
{
    public static class UserExtensions
    {
        // Key of the account that signed in; null when nobody did.
        public static Guid? GetUserKey(this ClaimsPrincipal user) =>
            Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid key) ? key : null;
    }
}
