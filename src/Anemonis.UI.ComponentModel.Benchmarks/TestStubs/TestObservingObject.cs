using System.ComponentModel;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestStubs
{
    internal sealed class TestObservingObject : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
