using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public sealed class DataEventBrokerBenchmarks
    {
        private readonly DataEventBroker _broker = new DataEventBroker();

        public DataEventBrokerBenchmarks()
        {
            _broker.Subscribe<object>("pipe", OnDataEvent);
        }

        private void OnDataEvent(DataEventArgs<object> args)
        {
        }

        [Benchmark(Description = "Publish")]
        public void Publish()
        {
            _broker.Publish<object>("pipe", null);
        }
    }
}