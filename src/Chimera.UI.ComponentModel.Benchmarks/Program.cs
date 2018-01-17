using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Validators;
using Chimera.UI.ComponentModel.Benchmarks.Framework;

namespace Chimera.UI.ComponentModel.Benchmarks
{
    public static class Program
    {
        public static void Main()
        {
            var configuration = ManualConfig.CreateEmpty();

            configuration.Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            configuration.Add(Job.Dry.With(RunStrategy.Throughput).WithTargetCount(5));
            configuration.Add(ConsoleLogger.Default);
            configuration.Add(MemoryDiagnoser.Default);
            configuration.Add(JitOptimizationsValidator.DontFailOnError);

            BenchmarkSuiteRunner.Run(typeof(Program).Assembly, configuration);
        }
    }
}