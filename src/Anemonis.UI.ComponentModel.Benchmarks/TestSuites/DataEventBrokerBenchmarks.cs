using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public sealed class DataEventBrokerBenchmarks
    {
        private readonly IDataEventBroker _broker = new DataEventBroker();

        public DataEventBrokerBenchmarks()
        {
            _broker.Subscribe<object>("channel-name", e => { });
        }

        [Benchmark(Description = "Publish")]
        public void GetByField()
        {
            _broker.Publish<object>("channel-name", null);
        }
    }
}