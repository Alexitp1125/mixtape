using Microsoft.AspNetCore.Authentication;
using Mixtape.Components;

namespace Mixtape.Accounts
{
    public static class AccountEndpoints
    {
        // POST /sign-out closes the session and goes back to the start.
        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(PageNames.SignOut, CloseSessionAsync);
            return endpoints;
        }

        private static async Task CloseSessionAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
            httpContext.Response.Redirect(PageNames.Home);
        }
    }
}
