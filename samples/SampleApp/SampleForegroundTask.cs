using Koren.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SampleApp
{
    class SampleForegroundTask : IForegroundTask
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SampleForegroundTask> _logger;

        public SampleForegroundTask(IConfiguration configuration,ILogger<SampleForegroundTask> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation("From Remote: " + _configuration.GetValue<string>("key1"));
            _logger.LogInformation("From Local: " + _configuration.GetValue<string>("key"));
            _logger.LogInformation("From Environment: " + _configuration.GetValue<string>("env"));

            await Task.Delay(-1, stoppingToken);
        }
    }
}
