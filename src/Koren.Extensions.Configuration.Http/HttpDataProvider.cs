using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Koren.Extensions.Configuration.Http
{

    class HttpDataProvider : IHttpDataProvider, IDisposable
    {

        private DefaultChangeToken _changeToken = new DefaultChangeToken();
        private readonly HttpClient _httpClient;
        private HttpStatusCode _lastStatus = 0;
        private readonly string _url;
        private CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private Task _probeTask = null;
        private string _lastTag = null;
        private TimeSpan _probeInterval;

        public HttpDataProvider(HttpClient httpClient,
                                string url,
                                TimeSpan? probeInterval)
        {
            _httpClient = httpClient;
            _url = url;

            if (_probeInterval != null)
            {
                _probeInterval = probeInterval.Value;
                _probeTask = Task.Run(ProbeLoop);
            }
            else
            {
                _probeTask = Task.CompletedTask;
            }

        }



        private async Task ProbeLoop()
        {
            await Task.Delay(_probeInterval);

            while (!_cancelToken.IsCancellationRequested)
            {
                try
                {
                    await ProbeAsync();
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed in probing http configuration: " + e.ToString());
                }
                finally
                {
                    if (!_cancelToken.IsCancellationRequested)
                        await Task.Delay(_probeInterval);
                }
            }
        }

        private async Task ProbeAsync()
        {
            using (var req = new HttpRequestMessage())
            {
                if (_lastTag != null)
                    req.Headers.TryAddWithoutValidation("If-None-Match", _lastTag);

                req.RequestUri = new Uri(_url);

                using (var response = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, _cancelToken.Token))
                {
                    if (response.StatusCode == _lastStatus)
                        return;

                    _lastStatus = response.StatusCode;

                    if (response.StatusCode == HttpStatusCode.NotModified)
                        return;

                    response.EnsureSuccessStatusCode();

                    var currentTag = response.Headers.GetValues("ETag").FirstOrDefault()?.Trim('"');

                    if (currentTag != null)
                    {
                        if (_lastTag != null)
                        {
                            var previousToken = Interlocked.Exchange(ref _changeToken, new DefaultChangeToken());
                            previousToken.OnChange();
                        }

                        _lastTag = currentTag;
                    }

                }
            }
        }

        public void Dispose()
        {
            _cancelToken.Cancel();
            _probeTask.Wait();
        }

        public async Task<HttpData> GetAsync()
        {

            using (var response = await _httpClient.GetAsync(_url))
            {
                response.EnsureSuccessStatusCode();

                var ms = new MemoryStream();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    await stream.CopyToAsync(ms);
                }

                ms.Position = 0;

                var length = response.Content.Headers.ContentLength;

                return new HttpData
                {
                    Data = ms,
                    Length = length,
                };
            }

        }

        public IChangeToken Watch()
        {
            return _changeToken;
        }

    }
}
