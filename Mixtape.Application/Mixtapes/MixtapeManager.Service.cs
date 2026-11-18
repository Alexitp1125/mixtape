using Mixtape.Shared.Artists;
using Mixtape.Shared.Mixtapes;

namespace Mixtape.Application.Mixtapes
{
    public sealed partial class MixtapeManager : IMixtapesService
    {
        Task<IReadOnlyList<ArtistInfo>> IMixtapesService.GetMineAsync(Guid userKey, CancellationToken cancellationToken) =>
            GetMineAsync(userKey, cancellationToken);

        Task<MixtapeResult> IMixtapesService.CreateAsync(Guid userKey, string name, string coverFileName, CancellationToken cancellationToken) =>
            CreateAsync(userKey, name, coverFileName, cancellationToken);

        Task<MixtapeResult> IMixtapesService.UpdateAsync(
            Guid userKey,
            string mixtapeSlug,
            string name,
            string coverFileName,
            CancellationToken cancellationToken) =>
            UpdateAsync(userKey, mixtapeSlug, name, coverFileName, cancellationToken);

        Task<MixtapeResult> IMixtapesService.DeleteAsync(Guid userKey, string mixtapeSlug, CancellationToken cancellationToken) =>
            DeleteAsync(userKey, mixtapeSlug, cancellationToken);

        Task<MixtapeResult> IMixtapesService.AddTrackAsync(
            Guid userKey,
            string mixtapeSlug,
            string videoId,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken) =>
            AddTrackAsync(userKey, mixtapeSlug, videoId, title, performer, coverFileName, cancellationToken);

        Task<MixtapeResult> IMixtapesService.UpdateTrackAsync(
            Guid userKey,
            string trackSlug,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken) =>
            UpdateTrackAsync(userKey, trackSlug, title, performer, coverFileName, cancellationToken);

        Task<MixtapeResult> IMixtapesService.ShiftTrackAsync(Guid userKey, string trackSlug, bool up, CancellationToken cancellationToken) =>
            ShiftTrackAsync(userKey, trackSlug, up, cancellationToken);

        Task<MixtapeResult> IMixtapesService.MoveTrackAsync(
            Guid userKey,
            string trackSlug,
            string? targetSlug,
            string? newName,
            CancellationToken cancellationToken) =>
            MoveTrackAsync(userKey, trackSlug, targetSlug, newName, cancellationToken);

        Task<MixtapeResult> IMixtapesService.RemoveTrackAsync(Guid userKey, string trackSlug, CancellationToken cancellationToken) =>
            RemoveTrackAsync(userKey, trackSlug, cancellationToken);
    }
}
