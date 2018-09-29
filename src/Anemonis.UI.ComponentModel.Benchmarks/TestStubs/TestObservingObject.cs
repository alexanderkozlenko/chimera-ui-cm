using System.ComponentModel;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestStubs
{
    internal sealed class TestObservingObject : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}