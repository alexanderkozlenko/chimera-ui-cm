// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    public partial class BindableObject
    {
        private readonly struct SynchronizationContextAdapter
        {
            private readonly Action<string> _callback;
            private readonly string _propertyName;

            public SynchronizationContextAdapter(Action<string> callback, string propertyName)
            {
                _callback = callback;
                _propertyName = propertyName;
            }

            public void Post(object state)
            {
                _callback.Invoke(_propertyName);
            }
        }
    }
}