namespace Anemonis.UI.ComponentModel.Benchmarks.TestObjects
{
    internal sealed class TestTargetObject<T>
    {
        private T _value;

        public TestTargetObject(T value)
        {
            _value = value;
        }

        public T Value
        {
            get => _value;
            set => _value = value;
        }
    }
}