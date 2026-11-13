namespace Mixtape.Shared.Artists
{
    public interface IArtistsService
    {
        // All the artists, in the order of the catalog.
        Task<IReadOnlyList<ArtistInfo>> GetArtistsAsync(CancellationToken cancellationToken = default);

        // One artist by its slug, with its songs; null when it doesn't exist.
        Task<ArtistDetails?> GetArtistAsync(string slug, CancellationToken cancellationToken = default);
    }
}
