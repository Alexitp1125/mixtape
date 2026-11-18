namespace Mixtape.Shared.Videos
{
    // What YouTube said about a link.
    public enum VideoStatus
    {
        Ok,

        // The text isn't a link to a YouTube video.
        LinkInvalid,

        // The video doesn't exist or is private.
        NotFound,

        // Its owner doesn't allow playing it outside YouTube.
        NotEmbeddable,

        // YouTube didn't answer.
        Unavailable,
    }
}
