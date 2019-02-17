using Microsoft.Extensions.Configuration;
using System;

namespace Koren.Extensions.Configuration.Json
{
    public class JsonFileConfigurationSource : FileConfigurationSource
    {
        public bool ExpandEnvironmentVariables { get; set; }
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new JsonFileConfigurationProvider(this);
        }
    }
}