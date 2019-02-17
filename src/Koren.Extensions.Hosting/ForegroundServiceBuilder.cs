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

        public IForegroundServiceBuilder AddTask<TTask>() where TTask : class,IForegroundTask
        {
            Services.AddSingleton<IForegroundTask, TTask>();

            return this;

        }
    }
}
