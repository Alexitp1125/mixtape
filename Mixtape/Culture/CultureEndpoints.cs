using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Localization;

namespace Mixtape.Culture
{
    public static class CultureEndpoints
    {
        // GET /culture/es?redirectUri=/tracks/doma saves the language in a cookie and goes back to that page.
        public static IEndpointRouteBuilder MapCultureEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/culture/{culture}", SetCulture);
            return endpoints;
        }

        private static IResult SetCulture(string culture, string? redirectUri, HttpContext httpContext)
        {
            if (!SupportedCultures.All.Contains(culture))
            {
                return Results.NotFound();
            }

            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    MaxAge = TimeSpan.FromDays(365),
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true,
                });

            return Results.LocalRedirect(IsLocalPath(redirectUri) ? redirectUri : "/");
        }

        // Only paths of this site: "/x", never "//other.com" or "/\other.com".
        private static bool IsLocalPath([NotNullWhen(true)] string? path) =>
            !string.IsNullOrEmpty(path)
            && path[0] == '/'
            && (path.Length == 1 || (path[1] != '/' && path[1] != '\\'));
    }
}
