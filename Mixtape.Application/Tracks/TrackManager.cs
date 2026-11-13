using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mixtape.Data;
using Mixtape.Shared;
using Mixtape.Shared.Tracks;

namespace Mixtape.Application.Tracks
{
    // Reads the songs from the database.
    public sealed partial class TrackManager
    {
        private readonly IDbContextFactory<MixtapeDbContext> dbContextFactory;
        private readonly ILogger<TrackManager> logger;

        public TrackManager(
            IDbContextFactory<MixtapeDbContext> dbContextFactory,
            ILogger<TrackManager> logger)
        {
            this.dbContextFactory = dbContextFactory;
            this.logger = logger;
        }

        // All the songs: by artist, and inside each artist in its own order.
        public async Task<IReadOnlyList<TrackInfo>> GetTracksAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                return await dbContext.Tracks
                    .AsNoTracking()
                    .OrderBy(x => x.Artist.SortOrder)
                    .ThenBy(x => x.SortOrder)
                    .Select(x => new TrackInfo
                    {
                        Key = x.Key,
                        Slug = x.Slug,
                        Title = x.Title,
                        ArtistName = x.Artist.Name,
                        CoverFileName = x.Cover != null ? x.Cover.FileName : null,
                    })
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error listing tracks.", logger);
            }
        }

        // One song by its slug, with the slugs of the previous and next songs of its artist.
        public async Task<TrackDetails?> GetTrackAsync(string slug, CancellationToken cancellationToken = default)
        {
            try
            {
                string trackSlug = slug.ToLowerInvariant();

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                var track = await dbContext.Tracks
                    .AsNoTracking()
                    .Where(x => x.Slug == trackSlug)
                    .Select(x => new
                    {
                        x.Key,
                        x.ArtistKey,
                        x.Slug,
                        x.Title,
                        x.VideoId,
                        ArtistName = x.Artist.Name,
                        CoverFileName = x.Cover != null ? x.Cover.FileName : null,
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (track is null)
                {
                    return null;
                }

                List<string> slugs = await dbContext.Tracks
                    .AsNoTracking()
                    .Where(x => x.ArtistKey == track.ArtistKey)
                    .OrderBy(x => x.SortOrder)
                    .Select(x => x.Slug)
                    .ToListAsync(cancellationToken);

                int index = slugs.IndexOf(track.Slug);

                return new TrackDetails
                {
                    Key = track.Key,
                    Slug = track.Slug,
                    Title = track.Title,
                    ArtistName = track.ArtistName,
                    VideoId = track.VideoId,
                    CoverFileName = track.CoverFileName,
                    PreviousSlug = slugs[(index - 1 + slugs.Count) % slugs.Count],
                    NextSlug = slugs[(index + 1) % slugs.Count],
                };
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error loading the track.", logger);
            }
        }
    }
}
