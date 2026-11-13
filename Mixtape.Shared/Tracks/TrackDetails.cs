namespace Mixtape.Shared.Tracks
{
    // A song as its page and the player show it.
    public sealed class TrackDetails
    {
        public Guid Key { get; init; }

        public string Slug { get; init; } = null!;

        public string Title { get; init; } = null!;

        public string ArtistName { get; init; } = null!;

        // Name of its artist in addresses: /artists/{slug}.
        public string ArtistSlug { get; init; } = null!;

        // YouTube video ID (the part after v=).
        public string VideoId { get; init; } = null!;

        // File of its cover inside Content/covers; null when it has none.
        public string? CoverFileName { get; init; }

        // The songs before and after this one, among those of its artist. The list wraps around.
        public string PreviousSlug { get; init; } = null!;

        public string NextSlug { get; init; } = null!;
    }
}
