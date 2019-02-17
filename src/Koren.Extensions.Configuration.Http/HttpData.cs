using System.IO;

namespace Koren.Extensions.Configuration.Http
{
    public class HttpData
    {
        public long? Length { get; set; }
        public Stream Data { get; set; }
    }
}
