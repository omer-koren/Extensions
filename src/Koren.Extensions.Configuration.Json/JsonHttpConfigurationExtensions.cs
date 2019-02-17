using System;
using System.Net.Http;
using Koren.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

namespace Koren.Extensions.Configuration
{
    public static class JsonHttpConfigurationExtensions
    {
        public static IConfigurationBuilder AddJsonHttp(this IConfigurationBuilder builder, string url)
        {
            return AddJsonHttp(builder, httpClient: null, url: url, probeInterval: null, expandEnvironmentVariables: true);
        }

        
        public static IConfigurationBuilder AddJsonHttp(this IConfigurationBuilder builder, string url,TimeSpan probeInterval)
        {
            return AddJsonHttp(builder, httpClient: null, url: url, probeInterval: probeInterval, expandEnvironmentVariables: true);
        }

        public static IConfigurationBuilder AddJsonHttp(this IConfigurationBuilder builder, string url, TimeSpan probeInterval, bool expandEnvironmentVariables)
        {
            return AddJsonHttp(builder, httpClient: null, url: url, probeInterval: probeInterval, expandEnvironmentVariables: expandEnvironmentVariables);
        }

        public static IConfigurationBuilder AddJsonHttp(this IConfigurationBuilder builder,HttpClient httpClient,string url, TimeSpan? probeInterval, bool expandEnvironmentVariables)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"Invalid url {url}");
            }

            return builder.AddJsonHttp(s =>
            {
                s.HttpClient = httpClient;
                s.Url = url;
                s.ProbingInterval = probeInterval;
                s.ExpandEnvironmentVariables = expandEnvironmentVariables;
            });
        }

        public static IConfigurationBuilder AddJsonHttp(this IConfigurationBuilder builder, Action<JsonHttpConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}