// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ObservableSubscribeToken<T> : IDisposable
    {
        private readonly IObserver<T> _observer;
        private readonly Action<IObserver<T>> _callback;

        public ObservableSubscribeToken(IObserver<T> observer, Action<IObserver<T>> callback)
        {
            _observer = observer;
            _callback = callback;
        }

        public void Dispose()
        {
            _callback.Invoke(_observer);
        }
    }
}
