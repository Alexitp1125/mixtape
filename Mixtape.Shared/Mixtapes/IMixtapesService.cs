using Mixtape.Shared.Artists;

namespace Mixtape.Shared.Mixtapes
{
    public interface IMixtapesService
    {
        // The mixtapes of a person, in the order they were created.
        Task<IReadOnlyList<ArtistInfo>> GetMineAsync(Guid userKey, CancellationToken cancellationToken = default);

        // Creates a mixtape with a name (1 to 30 characters) and one of the generic cassette covers.
        Task<MixtapeResult> CreateAsync(Guid userKey, string name, string coverFileName, CancellationToken cancellationToken = default);

        // Adds a song to a mixtape of that person, with one of the generic song covers.
        Task<MixtapeResult> AddTrackAsync(
            Guid userKey,
            string mixtapeSlug,
            string videoId,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken = default);
    }
}
