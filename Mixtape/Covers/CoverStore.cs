namespace Mixtape.Covers
{
    // The cover SVGs of Content/covers, read once at startup.
    public sealed class CoverStore
    {
        private readonly Dictionary<string, string> covers;

        public CoverStore(IWebHostEnvironment environment)
        {
            string coversPath = Path.Combine(environment.ContentRootPath, "Content", "covers");

            covers = Directory.EnumerateFiles(coversPath, "*.svg").ToDictionary(
                x => Path.GetFileName(x)!,
                x => File.ReadAllText(x),
                StringComparer.OrdinalIgnoreCase);

            GenericTapes = covers.Keys.Where(x => x.StartsWith("generic-tape-", StringComparison.OrdinalIgnoreCase)).Order().ToList();
            GenericSongs = covers.Keys.Where(x => x.StartsWith("generic-song-", StringComparison.OrdinalIgnoreCase)).Order().ToList();
        }

        // File names of the covers a person can choose for a mixtape (cassettes).
        public IReadOnlyList<string> GenericTapes { get; }

        // File names of the covers a person can choose for a song.
        public IReadOnlyList<string> GenericSongs { get; }

        // Names of the cover files that exist.
        public IReadOnlySet<string> FileNames => covers.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

        // SVG markup of a cover; empty when there is no cover or the file doesn't exist.
        // With a title, a generic cover gets it written on it; the others don't change.
        public string GetCover(string? fileName, string? title = null)
        {
            string svg = fileName is not null && covers.TryGetValue(fileName, out string? found) ? found : "";

            return title is null ? svg : CoverTitle.Apply(svg, title);
        }
    }
}
