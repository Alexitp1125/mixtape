namespace Mixtape.Components
{
    // Routes of the app.
    public static class PageNames
    {
        public const string Home = "/";
        public const string Access = "/access";
        public const string SignOut = "/sign-out";
        public const string Examples = "/examples";
        public const string Artist = "/artists/{Slug}";
        public const string Track = "/tracks/{Slug}";
        public const string NotFound = "/not-found";
        public const string Error = "/error";

        // The access page, on the tab to create an account or on the one to sign in.
        public static string AccessUrl(bool create) => create ? "/access?mode=create" : "/access";

        public static string ArtistUrl(string slug) => $"/artists/{slug}";

        public static string TrackUrl(string slug) => $"/tracks/{slug}";
    }
}
