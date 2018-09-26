using Anemonis.UI.ComponentModel.Benchmarks.TestObjects;
using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public sealed class BindableCommandBenchmarks
    {
        private readonly TestObservableObject _observbleObject = new TestObservableObject();
        private readonly BindableCommand<object> _bindableCommand = new BindableCommand<object>(p => { });

        public BindableCommandBenchmarks()
        {
            _bindableCommand.SubscribePropertyChanged(_observbleObject);
        }

        [Benchmark(Description = "observable-PropertyChanged")]
        public void OnObservablePropertyChanged()
        {
            _observbleObject.RaisePropertyChanged("PropertyName");
        }
    }
}