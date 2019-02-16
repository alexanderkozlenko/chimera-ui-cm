using System;
using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public class BindableCommandBenchmarks
    {
        private readonly BindableCommand<object> _observableCommand0 = new BindableCommand<object>(p => { });
        private readonly BindableCommand<object> _observableCommand1 = new BindableCommand<object>(p => { });
        private readonly BindableCommand<object> _observableCommand2 = new BindableCommand<object>(p => { });

        public BindableCommandBenchmarks()
        {
            _observableCommand2.CanExecuteChanged += OnCanExecuteChanged;
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
        }

        private void CommandAction(object parameter)
        {
        }

        private bool CommandPredicate(object parameter)
        {
            return true;
        }

        [Benchmark(Description = "CanExecute")]
        public bool CanExecute1()
        {
            return _observableCommand0.CanExecute(null);
        }

        [Benchmark(Description = "Execute")]
        public void Execute()
        {
            _observableCommand0.Execute(null);
        }

        [Benchmark(Description = "RaiseCanExecuteChanged-NPC=N")]
        public void RaisePropertyChanged()
        {
            _observableCommand1.RaiseCanExecuteChanged();
        }

        [Benchmark(Description = "RaiseCanExecuteChanged-NPC=Y")]
        public void RaisePropertyChangedWithEventSubscriber()
        {
            _observableCommand2.RaiseCanExecuteChanged();
        }
    }
}