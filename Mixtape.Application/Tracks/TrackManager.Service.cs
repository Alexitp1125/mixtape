using Mixtape.Shared.Tracks;

namespace Mixtape.Application.Tracks
{
    public sealed partial class TrackManager : ITracksService
    {
        Task<IReadOnlyList<TrackInfo>> ITracksService.GetTracksAsync(CancellationToken cancellationToken) =>
            GetTracksAsync(cancellationToken);

        Task<TrackDetails?> ITracksService.GetTrackAsync(string slug, CancellationToken cancellationToken) =>
            GetTrackAsync(slug, cancellationToken);
    }
}
