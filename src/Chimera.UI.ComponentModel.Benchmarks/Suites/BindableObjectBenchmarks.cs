using BenchmarkDotNet.Attributes;
using Chimera.UI.ComponentModel.Benchmarks.Framework;
using Chimera.UI.ComponentModel.Benchmarks.Objects;

namespace Chimera.UI.ComponentModel.Benchmarks.Suites
{
    [BenchmarkSuite(nameof(BindableObject))]
    public abstract class BindableObjectBenchmarks
    {
        private readonly BindableObjectType1<bool> _type1Object = new BindableObjectType1<bool>();
        private readonly BindableObjectType2<bool> _type2ObjectWithNotNullTarget = new BindableObjectType2<bool>(new ValueObject<bool>());
        private readonly BindableObjectType2<bool> _type2ObjectWithNullTarget = new BindableObjectType2<bool>(null);

        [Benchmark(Description = "T1 GET *** **")]
        public bool GetFromType1()
        {
            return _type1Object.Value;
        }

        [Benchmark(Description = "T1 SET *** ==")]
        public void SetToType1TheSameValue()
        {
            _type1Object.Value = _type1Object.Source;
        }

        [Benchmark(Description = "T1 SET *** !=")]
        public void SetToType1DifferentValue()
        {
            _type1Object.Value = !_type1Object.Source;
        }

        [Benchmark(Description = "T2 GET NUL **")]
        public bool GetFromType2WithNullTarget()
        {
            return _type2ObjectWithNullTarget.Value;
        }

        [Benchmark(Description = "T2 SET NUL **")]
        public void SetToType2WithNullTarget()
        {
            _type2ObjectWithNullTarget.Value = true;
        }

        [Benchmark(Description = "T2 GET OBJ **")]
        public bool GetFromType2WithNotNullTarget()
        {
            return _type2ObjectWithNotNullTarget.Value;
        }

        [Benchmark(Description = "T2 SET OBJ ==")]
        public void SetToType2WithNotNullTargetTheSameValue()
        {
            _type2ObjectWithNotNullTarget.Value = _type2ObjectWithNotNullTarget.Source;
        }

        [Benchmark(Description = "T2 SET OBJ !=")]
        public void SetToType2WithNotNullTargetDifferentValue()
        {
            _type2ObjectWithNotNullTarget.Value = !_type2ObjectWithNotNullTarget.Source;
        }
    }
}