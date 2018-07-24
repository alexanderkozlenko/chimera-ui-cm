using System;

namespace Chimera.UI.ComponentModel.UnitTests.TestObjects
{
    internal sealed class BindableObjectType2<T> : BindableObject
    {
        private readonly T _target;

        public BindableObjectType2(T target)
        {
            _target = target;
        }

        public TValue InvokeGetValue<TValue>(string propertyName, TValue defaultValue)
        {
            return GetValue(_target, propertyName, defaultValue);
        }

        public void InvokeSetValue<TValue>(string propertyName, TValue value, Action action, string outerPropertyName)
        {
            SetValue(_target, propertyName, value, action, outerPropertyName);
        }

        public T Value
        {
            get => _target;
        }
    }
}