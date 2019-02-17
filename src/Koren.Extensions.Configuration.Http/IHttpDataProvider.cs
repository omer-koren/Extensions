using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace Koren.Extensions.Configuration.Http
{
    public interface IHttpDataProvider
    {
        Task<HttpData> GetAsync();
        IChangeToken Watch();
    }
}
