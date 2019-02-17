using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;

namespace Koren.Extensions.Configuration.Http
{
    public abstract class HttpConfigurationSource : IConfigurationSource
    {
        public HttpConfigurationSource()
        {
            _provider = new Lazy<IHttpDataProvider>(CreateProvider);
        }

        public IHttpDataProvider HttpDataProvider { get { return _provider.Value; } }
        public HttpClient HttpClient { get; set; }
        public string Url { get; set; }
        public TimeSpan? ProbingInterval { get; set; }
        public abstract IConfigurationProvider Build(IConfigurationBuilder builder);

        private Lazy<IHttpDataProvider> _provider;
        private IHttpDataProvider CreateProvider()
        {
            return new HttpDataProvider(HttpClient ?? new HttpClient(), Url, ProbingInterval);
        }
    }
}
