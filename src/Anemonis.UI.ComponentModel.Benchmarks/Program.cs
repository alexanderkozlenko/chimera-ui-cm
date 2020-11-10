using System.Linq;

using Anemonis.UI.ComponentModel.Benchmarks.Framework;
using Anemonis.UI.ComponentModel.Benchmarks.TestSuites;

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

using Perfolizer.Horology;

var configuration = ManualConfig.CreateEmpty();

configuration.AddJob(Job.Default
    .WithWarmupCount(1)
    .WithIterationTime(TimeInterval.FromMilliseconds(250))
    .WithMinIterationCount(15)
    .WithMaxIterationCount(20)
    .WithToolchain(InProcessEmitToolchain.Instance));
configuration.AddDiagnoser(MemoryDiagnoser.Default);
configuration.AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
configuration.AddLogger(ConsoleLogger.Default);
configuration.AddExporter(new SimpleBenchmarkExporter());
configuration.SummaryStyle = SummaryStyle.Default
    .WithTimeUnit(TimeUnit.Nanosecond)
    .WithSizeUnit(SizeUnit.B);

BenchmarkRunner.Run<BindableObjectBenchmarks>(configuration);
BenchmarkRunner.Run<BindableCommandBenchmarks>(configuration);
BenchmarkRunner.Run<ObservableObjectBenchmarks>(configuration);
BenchmarkRunner.Run<ObservableCommandBenchmarks>(configuration);
BenchmarkRunner.Run<DataEventBrokerBenchmarks>(configuration);
