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

        Task<MixtapeResult> IMixtapesService.AddTrackAsync(
            Guid userKey,
            string mixtapeSlug,
            string videoId,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken) =>
            AddTrackAsync(userKey, mixtapeSlug, videoId, title, performer, coverFileName, cancellationToken);
    }
}
