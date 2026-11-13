using Mixtape.Shared.Artists;

namespace Mixtape.Application.Artists
{
    public sealed partial class ArtistManager : IArtistsService
    {
        Task<IReadOnlyList<ArtistInfo>> IArtistsService.GetArtistsAsync(CancellationToken cancellationToken) =>
            GetArtistsAsync(cancellationToken);

        Task<ArtistDetails?> IArtistsService.GetArtistAsync(string slug, CancellationToken cancellationToken) =>
            GetArtistAsync(slug, cancellationToken);
    }
}
