// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ObservableSubscribeToken<T> : IDisposable
    {
        private IObserver<T> _observer;
        private Action<IObserver<T>> _callback;

        public ObservableSubscribeToken(IObserver<T> observer, Action<IObserver<T>> callback)
        {
            _observer = observer;
            _callback = callback;
        }

        public void Dispose()
        {
            _callback?.Invoke(_observer);
            _callback = null;
            _observer = null;
        }
    }
}
