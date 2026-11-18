using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Mixtape.Shared;
using Mixtape.Shared.Videos;

namespace Mixtape.Application.Videos
{
    // Asks YouTube about a video through its oEmbed service (no key needed).
    public sealed partial class VideoManager
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<VideoManager> logger;

        public VideoManager(HttpClient httpClient, ILogger<VideoManager> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        // Finds the video of a link. YouTube answers 401 when its owner doesn't allow playing it outside YouTube.
        public async Task<VideoResult> FindAsync(string link, CancellationToken cancellationToken = default)
        {
            try
            {
                string? videoId = GetVideoId(link);

                if (videoId is null)
                {
                    return Failed(VideoStatus.LinkInvalid);
                }

                string videoUrl = Uri.EscapeDataString($"https://www.youtube.com/watch?v={videoId}");

                using HttpResponseMessage response = await httpClient.GetAsync(
                    $"https://www.youtube.com/oembed?format=json&url={videoUrl}",
                    cancellationToken);

                if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                {
                    return Failed(VideoStatus.NotEmbeddable);
                }

                if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
                {
                    return Failed(VideoStatus.NotFound);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return Failed(VideoStatus.Unavailable);
                }

                OEmbedVideo? video = await response.Content.ReadFromJsonAsync<OEmbedVideo>(cancellationToken);

                return new VideoResult
                {
                    Status = VideoStatus.Ok,
                    VideoId = videoId,
                    Title = video?.Title ?? "",
                    Author = video?.AuthorName ?? "",
                };
            }
            catch (Exception ex)
            when (ex is HttpRequestException or TaskCanceledException)
            {
                logger.LogWarning(ex, "YouTube didn't answer.");
                return Failed(VideoStatus.Unavailable);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error looking up the video.", logger);
            }
        }

        // The video ID of a YouTube link (youtu.be/ID, watch?v=ID, /embed/ID, /shorts/ID, /live/ID); null when it isn't one.
        private static string? GetVideoId(string link)
        {
            if (!Uri.TryCreate(link.Trim(), UriKind.Absolute, out Uri? uri)
                || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
            {
                return null;
            }

            string host = uri.Host.ToLowerInvariant();

            if (host != "youtu.be" && host != "youtube.com" && !host.EndsWith(".youtube.com", StringComparison.Ordinal))
            {
                return null;
            }

            string pattern = host == "youtu.be"
                ? "^/([A-Za-z0-9_-]{11})(?![A-Za-z0-9_-])"
                : "(?:[?&]v=|/embed/|/shorts/|/live/)([A-Za-z0-9_-]{11})(?![A-Za-z0-9_-])";

            Match match = Regex.Match(uri.PathAndQuery, pattern);

            return match.Success ? match.Groups[1].Value : null;
        }

        private static VideoResult Failed(VideoStatus status) =>
            new() { Status = status };

        // What the oEmbed service answers (only what is used).
        private sealed class OEmbedVideo
        {
            [JsonPropertyName("title")]
            public string? Title { get; init; }

            [JsonPropertyName("author_name")]
            public string? AuthorName { get; init; }
        }
    }
}
