using System.Linq;
using Anemonis.UI.ComponentModel.Benchmarks.Framework;
using Anemonis.UI.ComponentModel.Benchmarks.TestSuites;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Anemonis.UI.ComponentModel.Benchmarks
{
    public static class Program
    {
        public static void Main()
        {
            var configuration = ManualConfig.CreateEmpty();

            configuration.Add(Job.Default
                .WithWarmupCount(1)
                .WithIterationTime(TimeInterval.FromMilliseconds(250))
                .WithMinIterationCount(15)
                .WithMaxIterationCount(20)
                .With(InProcessEmitToolchain.Instance));
            configuration.Add(MemoryDiagnoser.Default);
            configuration.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.Add(ConsoleLogger.Default);
            configuration.Add(new SimpleBenchmarkExporter());
            configuration.SummaryStyle = SummaryStyle.Default
                .WithTimeUnit(TimeUnit.Nanosecond)
                .WithSizeUnit(SizeUnit.B);

            BenchmarkRunner.Run<BindableObjectBenchmarks>(configuration);
            BenchmarkRunner.Run<BindableCommandBenchmarks>(configuration);
            BenchmarkRunner.Run<ObservableObjectBenchmarks>(configuration);
            BenchmarkRunner.Run<ObservableCommandBenchmarks>(configuration);
            BenchmarkRunner.Run<DataEventBrokerBenchmarks>(configuration);
        }
    }
}