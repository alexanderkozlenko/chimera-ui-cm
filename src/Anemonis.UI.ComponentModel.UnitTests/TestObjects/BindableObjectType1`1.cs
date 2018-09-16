using System;

namespace Anemonis.UI.ComponentModel.UnitTests.TestObjects
{
    internal sealed class BindableObjectType1<T> : BindableObject
    {
        private T _value;

        public BindableObjectType1(T value)
        {
            _value = value;
        }

        public T InvokeGetValue()
        {
            return GetValue(ref _value);
        }

        public void InvokeSetValue(T value, Action action, string outerPropertyName)
        {
            SetValue(ref _value, value, action, outerPropertyName);
        }

        public T Value
        {
            get => _value;
        }
    }
}