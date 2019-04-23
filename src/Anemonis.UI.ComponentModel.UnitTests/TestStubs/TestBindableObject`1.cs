using System;

namespace Anemonis.UI.ComponentModel.UnitTests.TestStubs
{
    internal sealed class TestBindableObject<T> : BindableObject
    {
        private readonly TestTargetObject<T> _target;
        private readonly Action _callback;

        private T _value;

        public TestBindableObject(T value, Action callback = null)
        {
            _value = value;
            _callback = callback;
        }

        public TestBindableObject(TestTargetObject<T> target, Action callback = null)
        {
            _target = target;
            _callback = callback;
        }

        public void InvokeRaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }

        private void OnValueUpdated()
        {
            _callback?.Invoke();
        }

        public T FieldValue
        {
            get => _value;
            set => _value = value;
        }

        public T BindableFieldValue
        {
            get => GetValue(ref _value);
            set => SetValue(ref _value, value, OnValueUpdated);
        }

        public T PropertyValue
        {
            get => _target.Value;
            set => _target.Value = value;
        }

        public T BindablePropertyValue
        {
            get => GetValue(_target, nameof(_target.Value), default(T));
            set => SetValue(_target, nameof(_target.Value), value, OnValueUpdated);
        }

        public T InvalidBindablePropertyValue
        {
            get => GetValue(_target, nameof(_target.Value) + "?", default(T));
            set => SetValue(_target, nameof(_target.Value) + "?", value, OnValueUpdated);
        }
    }
}
