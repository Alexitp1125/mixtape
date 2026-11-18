using Mixtape.Shared.Videos;

namespace Mixtape.Application.Videos
{
    public sealed partial class VideoManager : IVideosService
    {
        Task<VideoResult> IVideosService.FindAsync(string link, CancellationToken cancellationToken) =>
            FindAsync(link, cancellationToken);
    }
}
