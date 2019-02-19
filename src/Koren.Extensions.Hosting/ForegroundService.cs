using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Koren.Extensions.Hosting
{
    public class ForegroundService : BackgroundService
    {
        private readonly IEnumerable<IForegroundTask> _tasks;
        private readonly IApplicationLifetime _lifetime;

        public ForegroundService(IEnumerable<IForegroundTask> tasks,IApplicationLifetime lifetime)
        {
            _tasks = tasks;
            _lifetime = lifetime;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var waitTasks =
                _tasks.Select(x => Task.Run(async () => await x.ExecuteAsync(stoppingToken)))
                .ToArray();


                await Task.WhenAll(waitTasks);
            }
            finally
            {
                _lifetime.StopApplication();
            }
        }
    }
}
