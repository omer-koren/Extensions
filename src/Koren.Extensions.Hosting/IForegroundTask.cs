using System.Threading;
using System.Threading.Tasks;

namespace Koren.Extensions.Hosting
{
    public interface IForegroundTask
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
