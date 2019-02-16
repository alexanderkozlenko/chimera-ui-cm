using System;
using System.ComponentModel;
using Anemonis.UI.ComponentModel.Benchmarks.TestStubs;
using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public class ObservableCommandBenchmarks
    {
        private readonly PropertyChangedEventArgs _observingObjectEventArgs = new PropertyChangedEventArgs("Value");
        private readonly TestObservingObject _observingObject = new TestObservingObject();
        private readonly TestObserverObject<EventArgs> _observerObject = new TestObserverObject<EventArgs>();
        private readonly ObservableCommand<object> _observableCommand = new ObservableCommand<object>(p => { });
        private readonly ObservableCommand<object> _observableCommand0 = new ObservableCommand<object>(p => { });
        private readonly ObservableCommand<object> _observableCommand1 = new ObservableCommand<object>(p => { });
        private readonly ObservableCommand<object> _observableCommand2 = new ObservableCommand<object>(p => { });
        private readonly ObservableCommand<object> _observableCommand3 = new ObservableCommand<object>(p => { });
        private readonly ObservableCommand<object> _observableCommand4 = new ObservableCommand<object>(p => { });

        public ObservableCommandBenchmarks()
        {
            _observableCommand1.CanExecuteChanged += OnCanExecuteChanged;
            _observableCommand2.Subscribe(_observerObject);
            _observableCommand3.CanExecuteChanged += OnCanExecuteChanged;
            _observableCommand3.Subscribe(_observerObject);
            _observableCommand4.Subscribe(_observingObject);
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
        }

        [Benchmark(Description = "Subscribe-Unsubscribe")]
        public void SubscribeUnsubscribe()
        {
            _observableCommand.Subscribe(_observerObject).Dispose();
        }

        [Benchmark(Description = "RaiseCanExecuteChanged-NPC=N-OBS=N")]
        public void RaisePropertyChanged()
        {
            _observableCommand0.RaiseCanExecuteChanged();
        }

        [Benchmark(Description = "RaiseCanExecuteChanged-NPC=Y-OBS=N")]
        public void RaisePropertyChangedWithEventSubscriber()
        {
            _observableCommand1.RaiseCanExecuteChanged();
        }

        [Benchmark(Description = "RaiseCanExecuteChanged-NPC=N-OBS=Y")]
        public void RaisePropertyChangedWithObserverSubscriber()
        {
            _observableCommand2.RaiseCanExecuteChanged();
        }

        [Benchmark(Description = "RaiseCanExecuteChanged-NPC=Y-OBS=Y")]
        public void RaisePropertyChangedWithEventAndObserverSubscribers()
        {
            _observableCommand3.RaiseCanExecuteChanged();
        }

        [Benchmark(Description = "CanExecuteChanged-PropertyChanged")]
        public void CanExecuteChangedByNotifyPropertyChanged()
        {
            _observingObject.RaisePropertyChanged(_observingObject, _observingObjectEventArgs);
        }
    }
}