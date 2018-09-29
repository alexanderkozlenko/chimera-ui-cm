// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ObservableObjectSubscribeToken : IDisposable
    {
        private readonly object _syncRoot = new object();

        private ObservableObject _observable;
        private IObserver<PropertyChangedEventArgs> _observer;

        public ObservableObjectSubscribeToken(ObservableObject observable, IObserver<PropertyChangedEventArgs> observer)
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