namespace Mixtape.Shared.Tracks
{
    // A song as the catalog shows it.
    public sealed class TrackInfo
    {
        public Guid Key { get; init; }

        public string Slug { get; init; } = null!;

        public string Title { get; init; } = null!;

        public string ArtistName { get; init; } = null!;

        // File of its cover inside Content/covers; null when it has none.
        public string? CoverFileName { get; init; }
    }
}
