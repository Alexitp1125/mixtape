namespace Mixtape.Components
{
    // Routes of the app.
    public static class PageNames
    {
        public const string Home = "/";
        public const string Access = "/access";
        public const string SignOut = "/sign-out";
        public const string Account = "/account";

        // The account page, right after changing the PIN.
        public const string AccountPinChanged = "/account?pin=changed";

        public const string Examples = "/examples";
        public const string NewMixtape = "/mixtapes/new";
        public const string Artist = "/artists/{Slug}";
        public const string AddTrack = "/artists/{Slug}/add";
        public const string EditMixtape = "/artists/{Slug}/edit";
        public const string Track = "/tracks/{Slug}";
        public const string MoveTrack = "/tracks/{Slug}/move";
        public const string EditTrack = "/tracks/{Slug}/edit";
        public const string NotFound = "/not-found";
        public const string Error = "/error";

        // The access page, on the tab to create an account or on the one to sign in.
        public static string AccessUrl(bool create) => create ? "/access?mode=create" : "/access";

        public static string ArtistUrl(string slug) => $"/artists/{slug}";

        // The page of a mixtape, opened where one of its songs is (MixtapeView gives each song that id).
        public static string ArtistUrl(string slug, string trackSlug) => $"/artists/{slug}#{TrackId(trackSlug)}";

        // Id of a song inside the page of its mixtape.
        public static string TrackId(string trackSlug) => $"track-{trackSlug}";

        public static string EditMixtapeUrl(string slug) => $"/artists/{slug}/edit";

        public static string AddTrackUrl(string slug) => $"/artists/{slug}/add";

        public static string TrackUrl(string slug) => $"/tracks/{slug}";

        public static string MoveTrackUrl(string slug) => $"/tracks/{slug}/move";

        public static string EditTrackUrl(string slug) => $"/tracks/{slug}/edit";
    }
}
