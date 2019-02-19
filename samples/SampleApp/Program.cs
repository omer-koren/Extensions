using Microsoft.Extensions.Hosting;
using System;
using Koren.Extensions.Configuration;
using Koren.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var remoteAppSettings = 
                @"https://koren-extensions.s3.amazonaws.com/appsettings.remote.json?AWSAccessKeyId=AKIAI5P2V7YVB7JUL6DA&Signature=iU3zMdM7WXkF%2B%2F%2FhlpELaHsp9Xk%3D&Expires=1582106148";

            var host = new HostBuilder()
                    .ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.json")
                                                     .AddJsonHttp(remoteAppSettings, TimeSpan.FromSeconds(30)))
                    .ConfigureServices(x =>
                    {
                        x.AddForegroundHostedService()
                            .AddTask<SampleForegroundTask>();
                    })
                    .ConfigureLogging(x => x.AddConsole())
                    .Build();

            host.Run();
        }
    }
}
