using Microsoft.EntityFrameworkCore;
using Mixtape.Application.Catalog;
using Mixtape.Application.Tracks;
using Mixtape.Covers;
using Mixtape.Data;
using Mixtape.Shared.Tracks;

namespace Mixtape.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMixtape(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccess(configuration);
            services.AddBusinessServices();
            return services;
        }

        // PostgreSQL. The connection string comes from user-secrets in local and from
        // the ConnectionStrings__Default variable on the server.
        private static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("The connection string \"Default\" is missing.");

            services.AddDbContextFactory<MixtapeDbContext>(options => options.UseNpgsql(connectionString));
        }

        private static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddSingleton<CoverStore>();
            services.AddTransient<CatalogManager>();
            services.AddTransient<ITracksService, TrackManager>();
        }
    }
}
