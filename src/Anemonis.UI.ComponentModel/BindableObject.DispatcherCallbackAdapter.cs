// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;

#pragma warning disable IDE0060

namespace Anemonis.UI.ComponentModel
{
    public partial class BindableObject
    {
        private readonly struct DispatcherCallbackAdapter
        {
            private readonly Func<string, PropertyChangedEventArgs> _callback;
            private readonly string _propertyName;

            public DispatcherCallbackAdapter(Func<string, PropertyChangedEventArgs> callback, string propertyName)
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

#pragma warning restore IDE0060