using System;

namespace Anemonis.UI.ComponentModel.UnitTests.TestStubs
{
    internal sealed class TestObserverObject<T> : IObserver<T>
        where T : EventArgs
    {
        private readonly Action<T> _handler;

        public TestObserverObject(Action<T> handler)
        {
            _handler = handler;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
            _handler?.Invoke(value);
        }
    }
}