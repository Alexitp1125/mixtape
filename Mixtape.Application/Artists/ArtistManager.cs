using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mixtape.Data;
using Mixtape.Shared;
using Mixtape.Shared.Artists;
using Mixtape.Shared.Tracks;

namespace Mixtape.Application.Artists
{
    // Reads the artists from the database.
    public sealed partial class ArtistManager
    {
        private readonly IDbContextFactory<MixtapeDbContext> dbContextFactory;
        private readonly ILogger<ArtistManager> logger;

        public ArtistManager(
            IDbContextFactory<MixtapeDbContext> dbContextFactory,
            ILogger<ArtistManager> logger)
        {
            this.dbContextFactory = dbContextFactory;
            this.logger = logger;
        }

        // All the artists, in the order of the catalog, each with its cover and its number of songs.
        public async Task<IReadOnlyList<ArtistInfo>> GetArtistsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                return await dbContext.Artists
                    .AsNoTracking()
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new ArtistInfo
                    {
                        Key = x.Key,
                        Slug = x.Slug,
                        Name = x.Name,
                        CoverFileName = x.Cover != null ? x.Cover.FileName : null,
                        TrackCount = x.Tracks.Count,
                    })
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error listing artists.", logger);
            }
        }

        // One artist by its slug, with its songs in their order.
        public async Task<ArtistDetails?> GetArtistAsync(string slug, CancellationToken cancellationToken = default)
        {
            try
            {
                string artistSlug = slug.ToLowerInvariant();

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                var artist = await dbContext.Artists
                    .AsNoTracking()
                    .Where(x => x.Slug == artistSlug)
                    .Select(x => new
                    {
                        x.Key,
                        x.Slug,
                        x.Name,
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (artist is null)
                {
                    return null;
                }

                List<TrackInfo> tracks = await dbContext.Tracks
                    .AsNoTracking()
                    .Where(x => x.ArtistKey == artist.Key)
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new TrackInfo
                    {
                        Key = x.Key,
                        Slug = x.Slug,
                        Title = x.Title,
                        ArtistName = x.Artist.Name,
                        CoverFileName = x.Cover != null ? x.Cover.FileName : null,
                    })
                    .ToListAsync(cancellationToken);

                return new ArtistDetails
                {
                    Key = artist.Key,
                    Slug = artist.Slug,
                    Name = artist.Name,
                    Tracks = tracks,
                };
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error loading the artist.", logger);
            }
        }
    }
}
