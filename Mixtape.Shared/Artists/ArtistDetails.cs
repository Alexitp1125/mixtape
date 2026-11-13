using Mixtape.Shared.Tracks;

namespace Mixtape.Shared.Artists
{
    // An artist with their songs, as the artist page shows it.
    public sealed class ArtistDetails
    {
        public Guid Key { get; init; }

        public string Slug { get; init; } = null!;

        public string Name { get; init; } = null!;

        // The songs of the artist, in their order.
        public IReadOnlyList<TrackInfo> Tracks { get; init; } = [];
    }
}
