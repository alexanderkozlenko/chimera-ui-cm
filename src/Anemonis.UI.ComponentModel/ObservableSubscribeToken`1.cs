// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ObservableSubscribeToken<T> : IDisposable
    {
        private IObserver<T> _observer;
        private Action<IObserver<T>> _unsubscribe;

        public ObservableSubscribeToken(IObserver<T> observer, Action<IObserver<T>> unsubscribe)
        {
            _observer = observer;
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            _unsubscribe?.Invoke(_observer);
            _unsubscribe = null;
            _observer = null;
        }
    }
}