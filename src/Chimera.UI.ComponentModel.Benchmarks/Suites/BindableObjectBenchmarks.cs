using BenchmarkDotNet.Attributes;
using Chimera.UI.ComponentModel.Benchmarks.Objects;

namespace Chimera.UI.ComponentModel.Benchmarks.Suites
{
    public abstract class BindableObjectBenchmarks
    {
        private readonly BindableObjectType1<bool> _type1Object = new BindableObjectType1<bool>();
        private readonly BindableObjectType2<bool> _type2Object1 = new BindableObjectType2<bool>(new ValueObject<bool>());
        private readonly BindableObjectType2<bool> _type2Object2 = new BindableObjectType2<bool>(null);

        [Benchmark(Description = "t1 get --- --")]
        public bool GetFromType1()
        {
            return _type1Object.Value;
        }

        [Benchmark(Description = "t1 set --- ==")]
        public void SetToType1TheSameValue()
        {
            _type1Object.Value = _type1Object.Source;
        }

        [Benchmark(Description = "t1 set --- !=")]
        public void SetToType1DifferentValue()
        {
            _type1Object.Value = !_type1Object.Source;
        }

        [Benchmark(Description = "t2 get nul --")]
        public bool GetFromType2WithNullTarget()
        {
            return _type2Object2.Value;
        }

        [Benchmark(Description = "t2 set nul --")]
        public void SetToType2WithNullTarget()
        {
            _type2Object2.Value = true;
        }

        [Benchmark(Description = "t2 get obj --")]
        public bool GetFromType2WithNotNullTarget()
        {
            return _type2Object1.Value;
        }

        [Benchmark(Description = "t2 set obj ==")]
        public void SetToType2WithNotNullTargetTheSameValue()
        {
            _type2Object1.Value = _type2Object1.Source;
        }

        [Benchmark(Description = "t2 set obj !=")]
        public void SetToType2WithNotNullTargetDifferentValue()
        {
            _type2Object1.Value = !_type2Object1.Source;
        }
    }
}