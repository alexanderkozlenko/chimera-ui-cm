namespace Anemonis.UI.ComponentModel.UnitTests.TestObjects
{
    internal sealed class ObservingObject<T> : BindableObject
    {
        private T _value;

        public T Value
        {
            set => SetValue(ref _value, value);
        }
    }
}