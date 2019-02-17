using Microsoft.Extensions.DependencyInjection;

namespace Koren.Extensions.Hosting
{
    public static class ForegroundServiceBuilderExtensions
    {
        public static IForegroundServiceBuilder AddTask<TTask>(this IForegroundServiceBuilder builder) where TTask : class, IForegroundTask
        {
            builder.Services.AddSingleton<IForegroundTask, TTask>();
            return builder;
        }
    }
}
