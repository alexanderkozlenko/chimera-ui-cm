using System;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestStubs
{
    internal sealed class TestObserverObject<T> : IObserver<T>
        where T : EventArgs
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(T value)
        {
        }
    }
}
