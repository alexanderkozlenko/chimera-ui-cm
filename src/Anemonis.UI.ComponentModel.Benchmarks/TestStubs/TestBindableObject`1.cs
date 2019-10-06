namespace Anemonis.UI.ComponentModel.Benchmarks.TestStubs
{
    internal sealed class TestBindableObject<T> : BindableObject
    {
        private readonly TestTargetObject<T> _target;

        private T _value;

        public TestBindableObject(T value)
        {
            _value = value;
        }

        public TestBindableObject(TestTargetObject<T> target)
        {
            _target = target;
        }

        public T FieldValue
        {
            get => _value;
            set => _value = value;
        }

        public T BindableFieldValue
        {
            get => GetValue(_value);
            set => SetValue(ref _value, value);
        }

        public T PropertyValue
        {
            get => _target.Value;
            set => _target.Value = value;
        }

        public T BindablePropertyValue
        {
            get => GetValue(_target, nameof(TestTargetObject<T>.Value), default(T));
            set => SetValue(_target, nameof(TestTargetObject<T>.Value), value);
        }
    }
}
