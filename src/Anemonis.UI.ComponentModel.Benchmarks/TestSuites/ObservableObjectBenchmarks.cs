using System.ComponentModel;
using Anemonis.UI.ComponentModel.Benchmarks.TestStubs;
using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public sealed class ObservableObjectBenchmarks
    {
        private readonly TestObserverObject<PropertyChangedEventArgs> _observerObject = new TestObserverObject<PropertyChangedEventArgs>();
        private readonly TestObservableObject _observableObject = new TestObservableObject();
        private readonly TestObservableObject _observableObject0 = new TestObservableObject();
        private readonly TestObservableObject _observableObject1 = new TestObservableObject();
        private readonly TestObservableObject _observableObject2 = new TestObservableObject();
        private readonly TestObservableObject _observableObject3 = new TestObservableObject();

        public ObservableObjectBenchmarks()
        {
            _observableObject1.PropertyChanged += OnPropertyChanged;
            _observableObject2.Subscribe(_observerObject);
            _observableObject3.PropertyChanged += OnPropertyChanged;
            _observableObject3.Subscribe(_observerObject);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        [Benchmark(Description = "Subscribe-Unsubscribe")]
        public void SubscribeUnsubscribe()
        {
            _observableObject.Subscribe(_observerObject).Dispose();
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=N-OBS=N")]
        public void RaisePropertyChanged()
        {
            _observableObject0.InvokeRaisePropertyChanged("Value");
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=Y-OBS=N")]
        public void RaisePropertyChangedWithEventSubscriber()
        {
            _observableObject1.InvokeRaisePropertyChanged("Value");
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=N-OBS=Y")]
        public void RaisePropertyChangedWithObserverSubscriber()
        {
            _observableObject2.InvokeRaisePropertyChanged("Value");
        }

        [Benchmark(Description = "RaisePropertyChanged-NPC=Y-OBS=Y")]
        public void RaisePropertyChangedWithEventAndObserverSubscribers()
        {
            _observableObject3.InvokeRaisePropertyChanged("Value");
        }
    }
}