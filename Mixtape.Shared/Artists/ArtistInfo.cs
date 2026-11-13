namespace Mixtape.Shared.Artists
{
    // An artist as the home page shows it.
    public sealed class ArtistInfo
    {
        public Guid Key { get; init; }

        public string Slug { get; init; } = null!;

        public string Name { get; init; } = null!;

        // File of its cover inside Content/covers; null when it has none.
        public string? CoverFileName { get; init; }

        // Number of songs of the artist.
        public int TrackCount { get; init; }
    }
}
