using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Koren.Extensions.Hosting
{
    public static class IServiceCollectionExtensions
    {
        public static IForegroundServiceBuilder AddForegroundHostedService(this IServiceCollection services)
        {
            services.AddHostedService<ForegroundService>();

            return new ForegroundServiceBuilder(services);
        }
    }
}
