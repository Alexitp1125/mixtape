using System.Text.Json;
using Mixtape.Application.Catalog;

namespace Mixtape.Catalog
{
    // Alexi's catalog: Content/catalog.json, the artists with their songs.
    public static class CatalogFile
    {
        public static IReadOnlyList<CatalogArtist> Read(IWebHostEnvironment environment)
        {
            string json = File.ReadAllText(Path.Combine(environment.ContentRootPath, "Content", "catalog.json"));
            return JsonSerializer.Deserialize<List<CatalogArtist>>(json, JsonSerializerOptions.Web) ?? [];
        }
    }
}
