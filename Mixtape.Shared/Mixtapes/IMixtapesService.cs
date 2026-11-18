using Mixtape.Shared.Artists;

namespace Mixtape.Shared.Mixtapes
{
    // Everything that changes a mixtape needs the key of who signed in: only its owner can.
    public interface IMixtapesService
    {
        // The mixtapes of a person, in the order they were created.
        Task<IReadOnlyList<ArtistInfo>> GetMineAsync(Guid userKey, CancellationToken cancellationToken = default);

        // Creates a mixtape with a name (1 to 30 characters) and one of the generic cassette covers.
        Task<MixtapeResult> CreateAsync(Guid userKey, string name, string coverFileName, CancellationToken cancellationToken = default);

        // Changes the name and the cover of a mixtape. Its slug stays the same.
        Task<MixtapeResult> UpdateAsync(
            Guid userKey,
            string mixtapeSlug,
            string name,
            string coverFileName,
            CancellationToken cancellationToken = default);

        // Deletes a mixtape with its songs.
        Task<MixtapeResult> DeleteAsync(Guid userKey, string mixtapeSlug, CancellationToken cancellationToken = default);

        // Adds a song to a mixtape, with one of the generic song covers.
        Task<MixtapeResult> AddTrackAsync(
            Guid userKey,
            string mixtapeSlug,
            string videoId,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken = default);

        // Changes the title, the artist and the cover of a song. The result has the slug of its mixtape.
        Task<MixtapeResult> UpdateTrackAsync(
            Guid userKey,
            string trackSlug,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken = default);

        // Moves a song one place up or down in its mixtape. The result has the slug of that mixtape.
        Task<MixtapeResult> ShiftTrackAsync(Guid userKey, string trackSlug, bool up, CancellationToken cancellationToken = default);

        // Moves a song to another mixtape of the same person (targetSlug) or, without one, to a new mixtape called newName.
        // The result has the slug of the mixtape that got the song.
        Task<MixtapeResult> MoveTrackAsync(
            Guid userKey,
            string trackSlug,
            string? targetSlug,
            string? newName,
            CancellationToken cancellationToken = default);

        // Takes a song out of its mixtape. The result has the slug of that mixtape.
        Task<MixtapeResult> RemoveTrackAsync(Guid userKey, string trackSlug, CancellationToken cancellationToken = default);
    }
}
