namespace Mixtape.Application.Catalog
{
    // A song of catalog.json.
    public sealed class CatalogTrack
    {
        public string Slug { get; init; } = null!;

        public string Title { get; init; } = null!;

        // Who performs the song, when it isn't the artist the song is listed under.
        public string? Performer { get; init; }

        // YouTube video ID (the part after v=).
        public string VideoId { get; init; } = null!;
    }
}
