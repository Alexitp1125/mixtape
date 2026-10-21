namespace Mixtape.Client.Tracks
{
    // One line of lyrics.
    public sealed class LyricLine
    {
        // Second of the song where the line starts. Null when the lyrics are not synced.
        public double? Time { get; init; }

        public string Text { get; init; } = null!;
    }
}
