
using Koren.Extensions.Configuration.Http;
using Microsoft.Extensions.Configuration;

namespace Koren.Extensions.Configuration.Json
{
    public class JsonHttpConfigurationSource : HttpConfigurationSource
    {
        public bool ExpandEnvironmentVariables { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new JsonHttpConfigurationProvider(this);
        }
    }
}