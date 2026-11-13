namespace Mixtape.Application.Catalog
{
    // An artist of catalog.json, with their songs.
    public sealed class CatalogArtist
    {
        public string Slug { get; init; } = null!;

        public string Name { get; init; } = null!;

        public List<CatalogTrack> Tracks { get; init; } = [];
    }
}
