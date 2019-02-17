using Microsoft.Extensions.DependencyInjection;

namespace Koren.Extensions.Hosting
{
    public interface IForegroundServiceBuilder
    {
        IServiceCollection Services { get; }
    }
}
