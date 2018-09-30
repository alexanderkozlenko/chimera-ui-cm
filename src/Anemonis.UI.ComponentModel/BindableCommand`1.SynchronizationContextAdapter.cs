// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    public partial class BindableCommand<T>
    {
        private readonly struct SynchronizationContextAdapter
        {
            private readonly Action _callback;

            public SynchronizationContextAdapter(Action callback)
            {
                _callback = callback;
            }

            public void Post(object state)
            {
                _callback.Invoke();
            }
        }
    }
}