namespace Chimera.UI.ComponentModel.Tests.Objects
{
    internal sealed class TrackingObject<T> : BindableObject
    {
        private T _value;

        public T Value
        {
            set => SetValue(ref _value, value);
        }
    }
}