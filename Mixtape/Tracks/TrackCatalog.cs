using System.Text.Json;
using Mixtape.Client.Tracks;

namespace Mixtape.Tracks
{
    // Songs and covers from the Content folder, read once at startup.
    public sealed class TrackCatalog
    {
        private readonly List<TrackDetails> tracks;
        private readonly Dictionary<string, string> covers;

        public TrackCatalog(IWebHostEnvironment environment)
        {
            string contentPath = Path.Combine(environment.ContentRootPath, "Content");

            string json = File.ReadAllText(Path.Combine(contentPath, "tracks.json"));
            tracks = JsonSerializer.Deserialize<List<TrackDetails>>(json, JsonSerializerOptions.Web) ?? [];

            covers = tracks.ToDictionary(
                x => x.Slug,
                x => ReadCover(contentPath, x.Slug),
                StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyList<TrackDetails> GetTracks() => tracks;

        public TrackDetails? GetTrack(string slug) =>
            tracks.FirstOrDefault(x => string.Equals(x.Slug, slug, StringComparison.OrdinalIgnoreCase));

        // The songs before and after this one. The list wraps around: after the last comes the first.
        public TrackDetails GetPrevious(TrackDetails track) =>
            tracks[(tracks.IndexOf(track) - 1 + tracks.Count) % tracks.Count];

        public TrackDetails GetNext(TrackDetails track) =>
            tracks[(tracks.IndexOf(track) + 1) % tracks.Count];

        // SVG markup of the cover; empty when the song has none.
        public string GetCover(string slug) => covers.GetValueOrDefault(slug, "");

        private static string ReadCover(string contentPath, string slug)
        {
            string path = Path.Combine(contentPath, "covers", slug + ".svg");
            return File.Exists(path) ? File.ReadAllText(path) : "";
        }
    }
}
