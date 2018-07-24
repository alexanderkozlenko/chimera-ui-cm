namespace Chimera.UI.ComponentModel.Benchmarks.TestObjects
{
    internal sealed class BindableObjectType2<T> : BindableObject
    {
        private readonly ValueObject<T> _target;

        public BindableObjectType2(ValueObject<T> target)
        {
            _target = target;
        }

        public T Source
        {
            get => _target.Value;
        }

        public T Value
        {
            get => GetValue(_target, nameof(_target.Value), default(T));
            set => SetValue(_target, nameof(_target.Value), value);
        }
    }
}