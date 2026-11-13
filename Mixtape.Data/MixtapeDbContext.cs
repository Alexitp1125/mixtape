using Microsoft.EntityFrameworkCore;
using Mixtape.Data.Entities;

namespace Mixtape.Data
{
    // The Mixtape database (PostgreSQL).
    public sealed class MixtapeDbContext : DbContext
    {
        public MixtapeDbContext(DbContextOptions<MixtapeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artists => Set<Artist>();

        public DbSet<Cover> Covers => Set<Cover>();

        public DbSet<Track> Tracks => Set<Track>();
    }
}
