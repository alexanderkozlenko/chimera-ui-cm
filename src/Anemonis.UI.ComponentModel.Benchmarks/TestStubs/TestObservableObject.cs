namespace Anemonis.UI.ComponentModel.Benchmarks.TestStubs
{
    internal sealed class TestObservableObject : ObservableObject
    {
        public void InvokeRaisePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }
    }
}