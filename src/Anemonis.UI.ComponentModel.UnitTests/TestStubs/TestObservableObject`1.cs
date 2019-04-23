namespace Anemonis.UI.ComponentModel.UnitTests.TestStubs
{
    internal sealed class TestObservableObject<T> : ObservableObject
    {
        public void InvokeRaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }
    }
}
