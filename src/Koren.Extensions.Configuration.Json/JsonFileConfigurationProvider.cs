using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Koren.Extensions.Configuration.Json
{

    public class JsonFileConfigurationProvider : FileConfigurationProvider
    {
        private readonly JsonFileConfigurationSource _source;

        public JsonFileConfigurationProvider(JsonFileConfigurationSource source) : base(source)
        {
            _source = source;
        }

        public override void Load(Stream stream)
        {

            Data = JsonConfigurationParser.Parse(stream,_source.ExpandEnvironmentVariables);

        }
    }
}
