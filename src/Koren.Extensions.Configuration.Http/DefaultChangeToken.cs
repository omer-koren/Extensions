using Microsoft.Extensions.Primitives;
using System;
using System.Threading;

namespace Koren.Extensions.Configuration.Http
{
    class DefaultChangeToken : IChangeToken
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public bool ActiveChangeCallbacks => true;

        public bool HasChanged => _cts.IsCancellationRequested;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => _cts.Token.Register(callback, state);

        public void OnChange() => _cts.Cancel();
    }
}
