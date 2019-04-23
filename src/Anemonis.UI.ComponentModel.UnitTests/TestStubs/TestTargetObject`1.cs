namespace Anemonis.UI.ComponentModel.UnitTests.TestStubs
{
    internal sealed class TestTargetObject<T>
    {
        public TestTargetObject(T value)
        {
            Value = value;
        }

        public T Value
        {
            get;
            set;
        }
    }
}
