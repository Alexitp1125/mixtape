using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mixtape.Data;
using Mixtape.Data.Entities;
using Mixtape.Shared;

namespace Mixtape.Application.Catalog
{
    // Puts Alexi's catalog (catalog.json) into the database.
    public sealed class CatalogManager
    {
        private readonly IDbContextFactory<MixtapeDbContext> dbContextFactory;
        private readonly ILogger<CatalogManager> logger;

        public CatalogManager(
            IDbContextFactory<MixtapeDbContext> dbContextFactory,
            ILogger<CatalogManager> logger)
        {
            this.dbContextFactory = dbContextFactory;
            this.logger = logger;
        }

        // Adds the artists and songs that are new and updates the ones that changed, matching by slug.
        // It never deletes. The order of the file is the order of the catalog.
        // coverFiles: names of the SVG files that exist; the cover of a song is "{slug}.svg".
        public async Task SyncAsync(
            IReadOnlyList<CatalogArtist> catalog,
            IReadOnlySet<string> coverFiles,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Dictionary<string, Artist> artists = await dbContext.Artists.ToDictionaryAsync(x => x.Slug, cancellationToken);
                Dictionary<string, Track> tracks = await dbContext.Tracks.ToDictionaryAsync(x => x.Slug, cancellationToken);
                Dictionary<string, Cover> covers = await dbContext.Covers.ToDictionaryAsync(x => x.Name, cancellationToken);

                foreach ((CatalogArtist catalogArtist, int artistOrder) in catalog.Select((x, i) => (x, i)))
                {
                    if (!artists.TryGetValue(catalogArtist.Slug, out Artist? artist))
                    {
                        artist = new Artist { Slug = catalogArtist.Slug };
                        artists.Add(artist.Slug, artist);
                        dbContext.Artists.Add(artist);
                    }

                    artist.Name = catalogArtist.Name;
                    artist.SortOrder = artistOrder;

                    foreach ((CatalogTrack catalogTrack, int trackOrder) in catalogArtist.Tracks.Select((x, i) => (x, i)))
                    {
                        if (!tracks.TryGetValue(catalogTrack.Slug, out Track? track))
                        {
                            track = new Track { Slug = catalogTrack.Slug };
                            tracks.Add(track.Slug, track);
                            dbContext.Tracks.Add(track);
                        }

                        track.Artist = artist;
                        track.Title = catalogTrack.Title;
                        track.VideoId = catalogTrack.VideoId;
                        track.SortOrder = trackOrder;
                        track.Cover = GetCover(dbContext, covers, coverFiles, catalogTrack.Slug);
                    }
                }

                int changes = await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Catalog synced: {Artists} artists, {Tracks} tracks, {Changes} changes.", artists.Count, tracks.Count, changes);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error syncing the catalog.", logger);
            }
        }

        // The cover drawn for a song: the file "{slug}.svg". Null when the file doesn't exist.
        private static Cover? GetCover(
            MixtapeDbContext dbContext,
            Dictionary<string, Cover> covers,
            IReadOnlySet<string> coverFiles,
            string slug)
        {
            string fileName = slug + ".svg";

            if (!coverFiles.Contains(fileName))
            {
                return null;
            }

            if (!covers.TryGetValue(slug, out Cover? cover))
            {
                cover = new Cover { Name = slug, IsGeneric = false };
                covers.Add(cover.Name, cover);
                dbContext.Covers.Add(cover);
            }

            cover.FileName = fileName;
            return cover;
        }
    }
}
