namespace Mixtape.Shared.Tracks
{
    public interface ITracksService
    {
        // One song by its slug; null when it doesn't exist.
        Task<TrackDetails?> GetTrackAsync(string slug, CancellationToken cancellationToken = default);
    }
}
