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
        }

        // Names of the cover files that exist.
        public IReadOnlySet<string> FileNames => covers.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);

        // SVG markup of a cover; empty when the song has no cover or the file doesn't exist.
        public string GetCover(string? fileName) =>
            fileName is not null && covers.TryGetValue(fileName, out string? svg) ? svg : "";
    }
}
