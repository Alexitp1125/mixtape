namespace Mixtape.Shared.Videos
{
    // A YouTube video found from its link.
    public sealed class VideoResult
    {
        public VideoStatus Status { get; init; }

        // YouTube video ID, when Status is Ok; null otherwise.
        public string? VideoId { get; init; }

        public string Title { get; init; } = "";

        // Name of the channel.
        public string Author { get; init; } = "";
    }
}
