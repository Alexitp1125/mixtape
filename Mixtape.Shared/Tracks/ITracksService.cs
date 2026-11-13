namespace Mixtape.Shared.Tracks
{
    public interface ITracksService
    {
        // All the songs, in the order of the catalog.
        Task<IReadOnlyList<TrackInfo>> GetTracksAsync(CancellationToken cancellationToken = default);

        // One song by its slug; null when it doesn't exist.
        Task<TrackDetails?> GetTrackAsync(string slug, CancellationToken cancellationToken = default);
    }
}
