using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Koren.Extensions.Configuration.Http
{
    public abstract class HttpConfigurationProvider : ConfigurationProvider
    {
        private readonly IDisposable _changeTokenRegistration;
        public HttpConfigurationProvider(HttpConfigurationSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Source = source;

            if (Source.ProbingInterval != null)
            {
                _changeTokenRegistration = ChangeToken.OnChange(
                    () => Source.HttpDataProvider.Watch(),
                    async () =>
                    {
                        await Load(reload: true);
                    });
            }
        }


        private async Task Load(bool reload)
        {
            var data = await Source.HttpDataProvider.GetAsync();

            if (reload)
            {
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            using (var stream = data.Data)
            {
                Load(stream);
            }

            OnReload();
        }

        public HttpConfigurationSource Source { get; }

        public abstract void Load(Stream stream);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            _changeTokenRegistration?.Dispose();
        }

        public override string ToString() => $"{GetType().Name} for '{Source.Url}')";

    }
}
