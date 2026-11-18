using Microsoft.EntityFrameworkCore;
using Mixtape.Application.Accounts;
using Mixtape.Application.Artists;
using Mixtape.Application.Catalog;
using Mixtape.Application.Mixtapes;
using Mixtape.Application.Tracks;
using Mixtape.Application.Videos;
using Mixtape.Covers;
using Mixtape.Data;
using Mixtape.Shared.Accounts;
using Mixtape.Shared.Artists;
using Mixtape.Shared.Mixtapes;
using Mixtape.Shared.Tracks;
using Mixtape.Shared.Videos;

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
            services.AddTransient<IAccountsService, AccountManager>();
            services.AddTransient<IArtistsService, ArtistManager>();
            services.AddTransient<IMixtapesService, MixtapeManager>();
            services.AddTransient<ITracksService, TrackManager>();

            // The questions to YouTube give up after 8 seconds.
            services.AddHttpClient<IVideosService, VideoManager>(x => x.Timeout = TimeSpan.FromSeconds(8));
        }
    }
}
