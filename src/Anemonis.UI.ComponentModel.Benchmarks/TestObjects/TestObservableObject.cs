using System.ComponentModel;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestObjects
{
    internal sealed class TestObservableObject : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}