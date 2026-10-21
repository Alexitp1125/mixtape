namespace Mixtape.Components
{
    // Routes of the app.
    public static class PageNames
    {
        public const string Home = "/";
        public const string Track = "/tracks/{Slug}";
        public const string NotFound = "/not-found";
        public const string Error = "/error";

        public static string TrackUrl(string slug) => $"/tracks/{slug}";
    }
}
