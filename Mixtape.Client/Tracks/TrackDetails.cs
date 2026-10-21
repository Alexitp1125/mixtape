namespace Mixtape.Client.Tracks
{
    // A song as the player shows it.
    public sealed class TrackDetails
    {
        public string Slug { get; init; } = null!;

        public string Title { get; init; } = null!;

        public string Artist { get; init; } = null!;

        // YouTube video ID (the part after v=).
        public string VideoId { get; init; } = null!;

        public IReadOnlyList<LyricLine> Lyrics { get; init; } = [];
    }
}
