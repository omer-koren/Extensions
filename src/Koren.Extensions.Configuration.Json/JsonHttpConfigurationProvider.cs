using System.IO;
using Koren.Extensions.Configuration.Http;

namespace Koren.Extensions.Configuration.Json
{
    public class JsonHttpConfigurationProvider : HttpConfigurationProvider
    {
        private readonly JsonHttpConfigurationSource _source;

        public JsonHttpConfigurationProvider(JsonHttpConfigurationSource source) : base(source)
        {
            this._source = source;
        }

        public override void Load(Stream stream)
        {
            Data = JsonConfigurationParser.Parse(stream,_source.ExpandEnvironmentVariables);
        }
    }
}
