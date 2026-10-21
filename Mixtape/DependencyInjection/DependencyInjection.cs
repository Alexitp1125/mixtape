using Mixtape.Tracks;

namespace Mixtape.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMixtape(this IServiceCollection services)
        {
            services.AddBusinessServices();
            return services;
        }

        private static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddSingleton<TrackCatalog>();
        }
    }
}
