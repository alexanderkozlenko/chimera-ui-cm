using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public class DataEventBrokerBenchmarks
    {
        private readonly DataEventBroker _broker = new();

        public DataEventBrokerBenchmarks()
        {
            _broker.Subscribe<object>("pipe-1", OnDataEvent);
        }

        private void OnDataEvent(DataEventArgs<object> args)
        {
        }

        [Benchmark(Description = "Subscribe-Unsubscribe")]
        public void SubscribeUnsubscribe()
        {
            _broker.Subscribe<object>("pipe", OnDataEvent);
            _broker.Unsubscribe<object>("pipe", OnDataEvent);
        }

        [Benchmark(Description = "Publish-Subscribers=0")]
        public void PublishSubscribers0()
        {
            _broker.Publish<object>("pipe-0", null);
        }

        [Benchmark(Description = "Publish-Subscribers=1")]
        public void PublishSubscribers1()
        {
            _broker.Publish<object>("pipe-1", null);
        }
    }
}
