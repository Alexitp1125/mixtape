namespace Mixtape.Shared.Videos
{
    public interface IVideosService
    {
        // Asks YouTube about a link: whether the video exists and can be played outside YouTube, its title and its channel.
        Task<VideoResult> FindAsync(string link, CancellationToken cancellationToken = default);
    }
}
