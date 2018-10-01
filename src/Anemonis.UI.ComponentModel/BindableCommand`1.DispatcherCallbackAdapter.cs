// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    public partial class BindableCommand<T>
    {
        private readonly struct DispatcherCallbackAdapter
        {
            private readonly Func<EventArgs> _callback;

            public DispatcherCallbackAdapter(Func<EventArgs> callback)
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