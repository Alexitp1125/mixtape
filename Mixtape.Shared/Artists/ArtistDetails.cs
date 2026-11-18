using Mixtape.Shared.Tracks;

namespace Mixtape.Shared.Artists
{
    // An artist of the catalog or a mixtape, with its songs, as its page shows it.
    public sealed class ArtistDetails
    {
        public Guid Key { get; init; }

        public string Slug { get; init; } = null!;

        public string Name { get; init; } = null!;

        // Account the mixtape belongs to; null for the artists of the catalog.
        public Guid? OwnerKey { get; init; }

        // The songs of the artist, in their order.
        public IReadOnlyList<TrackInfo> Tracks { get; init; } = [];
    }
}
