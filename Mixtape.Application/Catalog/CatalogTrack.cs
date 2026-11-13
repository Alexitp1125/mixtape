namespace Mixtape.Application.Catalog
{
    // A song of catalog.json.
    public sealed class CatalogTrack
    {
        public string Slug { get; init; } = null!;

        public string Title { get; init; } = null!;

        // YouTube video ID (the part after v=).
        public string VideoId { get; init; } = null!;
    }
}
