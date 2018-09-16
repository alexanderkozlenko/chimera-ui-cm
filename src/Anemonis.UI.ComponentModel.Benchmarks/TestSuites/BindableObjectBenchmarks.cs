using Anemonis.UI.ComponentModel.Benchmarks.TestObjects;
using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public sealed class BindableObjectBenchmarks
    {
        private readonly BindableObjectType1<bool> _type1Object = new BindableObjectType1<bool>();
        private readonly BindableObjectType2<bool> _type2Object1 = new BindableObjectType2<bool>(new ValueObject<bool>());
        private readonly BindableObjectType2<bool> _type2Object2 = new BindableObjectType2<bool>(null);

        [Benchmark(Description = "get-TYPE=T1-NULL=N")]
        public bool GetFromType1()
        {
            return _type1Object.Value;
        }

        [Benchmark(Description = "get-TYPE=T2-NULL=Y")]
        public bool GetFromType2TargetNull()
        {
            return _type2Object2.Value;
        }

        [Benchmark(Description = "get-TYPE=T2-NULL=N")]
        public bool GetFromType2()
        {
            return _type2Object1.Value;
        }

        [Benchmark(Description = "set-TYPE=T1-NULL=N-UPDATE=N")]
        public void SetToType1ValuesAreEqual()
        {
            _type1Object.Value = _type1Object.Source;
        }

        [Benchmark(Description = "set-TYPE=T1-NULL=N-UPDATE=Y")]
        public void SetToType1ValuesAreNotEqual()
        {
            _type1Object.Value = !_type1Object.Source;
        }

        [Benchmark(Description = "set-TYPE=T2-NULL=Y-UPDATE=N")]
        public void SetToType2TargetNull()
        {
            _type2Object2.Value = true;
        }

        [Benchmark(Description = "set-TYPE=T2-NULL=N-UPDATE=N")]
        public void SetToType2ValuesAreEqual()
        {
            _type2Object1.Value = _type2Object1.Source;
        }

        [Benchmark(Description = "set-TYPE=T2-NULL=N-UPDATE=Y")]
        public void SetToType2ValuesAreNotEqual()
        {
            _type2Object1.Value = !_type2Object1.Source;
        }
    }
}