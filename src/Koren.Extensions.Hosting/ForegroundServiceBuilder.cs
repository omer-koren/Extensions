using Microsoft.Extensions.DependencyInjection;

namespace Koren.Extensions.Hosting
{
    class ForegroundServiceBuilder : IForegroundServiceBuilder
    {
        public ForegroundServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

    }
}
