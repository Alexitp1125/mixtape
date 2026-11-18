using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mixtape.Data;
using Mixtape.Data.Entities;
using Mixtape.Shared;
using Mixtape.Shared.Artists;
using Mixtape.Shared.Mixtapes;

namespace Mixtape.Application.Mixtapes
{
    // The mixtapes of a person: listing, creating, changing and deleting them, and adding, changing, moving and removing their songs.
    // In the database a mixtape is an artist with an owner.
    public sealed partial class MixtapeManager
    {
        // Start of the file names of the generic covers: cassettes for mixtapes, the others for songs.
        private const string TapeCovers = "generic-tape-";
        private const string SongCovers = "generic-song-";

        private readonly IDbContextFactory<MixtapeDbContext> dbContextFactory;
        private readonly ILogger<MixtapeManager> logger;

        public MixtapeManager(
            IDbContextFactory<MixtapeDbContext> dbContextFactory,
            ILogger<MixtapeManager> logger)
        {
            this.dbContextFactory = dbContextFactory;
            this.logger = logger;
        }

        // The mixtapes of a person, in the order they were created.
        public async Task<IReadOnlyList<ArtistInfo>> GetMineAsync(Guid userKey, CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                return await dbContext.Artists
                    .AsNoTracking()
                    .Where(x => x.UserKey == userKey)
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
                throw ex.Handle("Error listing the mixtapes.", logger);
            }
        }

        // Creates a mixtape for a person.
        public async Task<MixtapeResult> CreateAsync(Guid userKey, string name, string coverFileName, CancellationToken cancellationToken = default)
        {
            try
            {
                string cleanName = name.Trim();

                if (cleanName.Length is < 1 or > 30)
                {
                    return Failed(MixtapeStatus.NameInvalid);
                }

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Cover? cover = await FindCoverAsync(dbContext, coverFileName, TapeCovers, cancellationToken);

                if (cover is null)
                {
                    return Failed(MixtapeStatus.CoverInvalid);
                }

                if (await IsFullOfMixtapesAsync(dbContext, userKey, cancellationToken))
                {
                    return Failed(MixtapeStatus.TooManyMixtapes);
                }

                // The account can be gone while a session of it is still open somewhere.
                if (!await dbContext.Users.AnyAsync(x => x.Key == userKey, cancellationToken))
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                Artist artist = new()
                {
                    Slug = Slugs.Create(cleanName),
                    Name = cleanName,
                    SortOrder = await NextMixtapeOrderAsync(dbContext, userKey, cancellationToken),
                    UserKey = userKey,
                    Cover = cover,
                };

                dbContext.Artists.Add(artist);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(artist.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error creating the mixtape.", logger);
            }
        }

        // Changes the name and the cover of a mixtape. Its slug stays the same, so the links already shared keep working.
        public async Task<MixtapeResult> UpdateAsync(
            Guid userKey,
            string mixtapeSlug,
            string name,
            string coverFileName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string cleanName = name.Trim();

                if (cleanName.Length is < 1 or > 30)
                {
                    return Failed(MixtapeStatus.NameInvalid);
                }

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Artist? artist = await FindMixtapeAsync(dbContext, userKey, mixtapeSlug, cancellationToken);

                if (artist is null)
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                Cover? cover = await FindCoverAsync(dbContext, coverFileName, TapeCovers, cancellationToken);

                if (cover is null)
                {
                    return Failed(MixtapeStatus.CoverInvalid);
                }

                artist.Name = cleanName;
                artist.Cover = cover;
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(artist.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error changing the mixtape.", logger);
            }
        }

        // Adds a song at the end of a mixtape. Only its owner can.
        public async Task<MixtapeResult> AddTrackAsync(
            Guid userKey,
            string mixtapeSlug,
            string videoId,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string cleanTitle = title.Trim();
                string cleanPerformer = performer.Trim();

                if (!Regex.IsMatch(videoId, "^[A-Za-z0-9_-]{11}$"))
                {
                    return Failed(MixtapeStatus.VideoInvalid);
                }

                if (cleanTitle.Length is < 1 or > 120)
                {
                    return Failed(MixtapeStatus.TitleInvalid);
                }

                if (cleanPerformer.Length is < 1 or > 120)
                {
                    return Failed(MixtapeStatus.PerformerInvalid);
                }

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Artist? artist = await FindMixtapeAsync(dbContext, userKey, mixtapeSlug, cancellationToken);

                if (artist is null)
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                Cover? cover = await FindCoverAsync(dbContext, coverFileName, SongCovers, cancellationToken);

                if (cover is null)
                {
                    return Failed(MixtapeStatus.CoverInvalid);
                }

                if (await IsFullOfTracksAsync(dbContext, artist.Key, cancellationToken))
                {
                    return Failed(MixtapeStatus.TooManyTracks);
                }

                Track track = new()
                {
                    Slug = Slugs.Create(cleanTitle),
                    Artist = artist,
                    Title = cleanTitle,
                    Performer = cleanPerformer,
                    VideoId = videoId,
                    SortOrder = await NextTrackOrderAsync(dbContext, artist.Key, cancellationToken),
                    Cover = cover,
                };

                dbContext.Tracks.Add(track);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(track.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error adding the song.", logger);
            }
        }

        // Changes the title, the artist and the cover of a song. The video stays the same.
        public async Task<MixtapeResult> UpdateTrackAsync(
            Guid userKey,
            string trackSlug,
            string title,
            string performer,
            string coverFileName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string cleanTitle = title.Trim();
                string cleanPerformer = performer.Trim();

                if (cleanTitle.Length is < 1 or > 120)
                {
                    return Failed(MixtapeStatus.TitleInvalid);
                }

                if (cleanPerformer.Length is < 1 or > 120)
                {
                    return Failed(MixtapeStatus.PerformerInvalid);
                }

                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Track? track = await FindTrackAsync(dbContext, userKey, trackSlug, cancellationToken);

                if (track is null)
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                Cover? cover = await FindCoverAsync(dbContext, coverFileName, SongCovers, cancellationToken);

                if (cover is null)
                {
                    return Failed(MixtapeStatus.CoverInvalid);
                }

                track.Title = cleanTitle;
                track.Performer = cleanPerformer;
                track.Cover = cover;
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(track.Artist.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error changing the song.", logger);
            }
        }

        // Moves a song one place up or down in its mixtape: it trades places with the song next to it.
        // The first song doesn't go up and the last one doesn't go down.
        public async Task<MixtapeResult> ShiftTrackAsync(Guid userKey, string trackSlug, bool up, CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Track? track = await FindTrackAsync(dbContext, userKey, trackSlug, cancellationToken);

                if (track is null)
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                Guid artistKey = track.ArtistKey;
                int order = track.SortOrder;

                // The song before it (to go up) or after it (to go down).
                Track? other = up
                    ? await dbContext.Tracks
                        .Where(x => x.ArtistKey == artistKey && x.SortOrder < order)
                        .OrderByDescending(x => x.SortOrder)
                        .FirstOrDefaultAsync(cancellationToken)
                    : await dbContext.Tracks
                        .Where(x => x.ArtistKey == artistKey && x.SortOrder > order)
                        .OrderBy(x => x.SortOrder)
                        .FirstOrDefaultAsync(cancellationToken);

                if (other is not null)
                {
                    track.SortOrder = other.SortOrder;
                    other.SortOrder = order;
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                return Succeeded(track.Artist.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error moving the song up or down.", logger);
            }
        }

        // Deletes a mixtape. The database deletes its songs with it.
        public async Task<MixtapeResult> DeleteAsync(Guid userKey, string mixtapeSlug, CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Artist? artist = await FindMixtapeAsync(dbContext, userKey, mixtapeSlug, cancellationToken);

                if (artist is null)
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                dbContext.Artists.Remove(artist);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(artist.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error deleting the mixtape.", logger);
            }
        }

        // Moves a song to the end of another mixtape of the same person or, without a target, of a new mixtape
        // (which gets the cover of the mixtape the song comes from).
        public async Task<MixtapeResult> MoveTrackAsync(
            Guid userKey,
            string trackSlug,
            string? targetSlug,
            string? newName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Track? track = await FindTrackAsync(dbContext, userKey, trackSlug, cancellationToken);

                if (track is null)
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                if (string.IsNullOrEmpty(targetSlug))
                {
                    string cleanName = (newName ?? "").Trim();

                    if (cleanName.Length is < 1 or > 30)
                    {
                        return Failed(MixtapeStatus.NameInvalid);
                    }

                    if (await IsFullOfMixtapesAsync(dbContext, userKey, cancellationToken))
                    {
                        return Failed(MixtapeStatus.TooManyMixtapes);
                    }

                    Artist created = new()
                    {
                        Slug = Slugs.Create(cleanName),
                        Name = cleanName,
                        SortOrder = await NextMixtapeOrderAsync(dbContext, userKey, cancellationToken),
                        UserKey = userKey,
                        CoverKey = track.Artist.CoverKey,
                    };

                    dbContext.Artists.Add(created);

                    track.Artist = created;
                    track.SortOrder = 0;
                    await dbContext.SaveChangesAsync(cancellationToken);

                    return Succeeded(created.Slug);
                }

                Artist? target = await dbContext.Artists.FirstOrDefaultAsync(
                    x => x.Slug == targetSlug && x.UserKey == userKey && x.Key != track.ArtistKey,
                    cancellationToken);

                if (target is null)
                {
                    return Failed(MixtapeStatus.TargetInvalid);
                }

                if (await IsFullOfTracksAsync(dbContext, target.Key, cancellationToken))
                {
                    return Failed(MixtapeStatus.TooManyTracks);
                }

                track.SortOrder = await NextTrackOrderAsync(dbContext, target.Key, cancellationToken);
                track.Artist = target;
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(target.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error moving the song.", logger);
            }
        }

        // Takes a song out of its mixtape.
        public async Task<MixtapeResult> RemoveTrackAsync(Guid userKey, string trackSlug, CancellationToken cancellationToken = default)
        {
            try
            {
                using MixtapeDbContext dbContext = dbContextFactory.CreateDbContext();

                Track? track = await FindTrackAsync(dbContext, userKey, trackSlug, cancellationToken);

                if (track is null)
                {
                    return Failed(MixtapeStatus.NotFound);
                }

                dbContext.Tracks.Remove(track);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Succeeded(track.Artist.Slug);
            }
            catch (Exception ex)
            when (ex is not AppException)
            {
                throw ex.Handle("Error removing the song.", logger);
            }
        }

        // A mixtape of that person; null when there is none.
        private static Task<Artist?> FindMixtapeAsync(
            MixtapeDbContext dbContext,
            Guid userKey,
            string mixtapeSlug,
            CancellationToken cancellationToken) =>
            dbContext.Artists.FirstOrDefaultAsync(x => x.Slug == mixtapeSlug && x.UserKey == userKey, cancellationToken);

        // A song of a mixtape of that person, with its mixtape; null when there is none.
        private static Task<Track?> FindTrackAsync(
            MixtapeDbContext dbContext,
            Guid userKey,
            string trackSlug,
            CancellationToken cancellationToken) =>
            dbContext.Tracks
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Slug == trackSlug && x.Artist.UserKey == userKey, cancellationToken);

        private static async Task<bool> IsFullOfMixtapesAsync(MixtapeDbContext dbContext, Guid userKey, CancellationToken cancellationToken) =>
            await dbContext.Artists.CountAsync(x => x.UserKey == userKey, cancellationToken) >= MixtapeLimits.MaxMixtapes;

        private static async Task<bool> IsFullOfTracksAsync(MixtapeDbContext dbContext, Guid artistKey, CancellationToken cancellationToken) =>
            await dbContext.Tracks.CountAsync(x => x.ArtistKey == artistKey, cancellationToken) >= MixtapeLimits.MaxTracks;

        // Position of a new mixtape: after the last one of that person.
        private static async Task<int> NextMixtapeOrderAsync(MixtapeDbContext dbContext, Guid userKey, CancellationToken cancellationToken) =>
            (await dbContext.Artists.Where(x => x.UserKey == userKey).MaxAsync(x => (int?)x.SortOrder, cancellationToken) ?? -1) + 1;

        // Position of a new song: after the last one of that mixtape.
        private static async Task<int> NextTrackOrderAsync(MixtapeDbContext dbContext, Guid artistKey, CancellationToken cancellationToken) =>
            (await dbContext.Tracks.Where(x => x.ArtistKey == artistKey).MaxAsync(x => (int?)x.SortOrder, cancellationToken) ?? -1) + 1;

        // A generic cover by its file name; null when it isn't one of that kind.
        private static async Task<Cover?> FindCoverAsync(
            MixtapeDbContext dbContext,
            string fileName,
            string kind,
            CancellationToken cancellationToken)
        {
            if (!fileName.StartsWith(kind, StringComparison.Ordinal))
            {
                return null;
            }

            return await dbContext.Covers.FirstOrDefaultAsync(x => x.IsGeneric && x.FileName == fileName, cancellationToken);
        }

        private static MixtapeResult Failed(MixtapeStatus status) =>
            new() { Status = status };

        private static MixtapeResult Succeeded(string slug) =>
            new() { Status = MixtapeStatus.Ok, Slug = slug };
    }
}
