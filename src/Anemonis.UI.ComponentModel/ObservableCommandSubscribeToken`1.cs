// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ObservableCommandSubscribeToken<T> : IDisposable
    {
        private readonly object _syncRoot = new object();

        private ObservableCommand<T> _observable;
        private IObserver<EventArgs> _observer;

        public ObservableCommandSubscribeToken(ObservableCommand<T> observable, IObserver<EventArgs> observer)
        {
            _observable = observable;
            _observer = observer;
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                _observable?.Unsubscribe(_observer);
                _observable = null;
                _observer = null;
            }
        }
    }
}