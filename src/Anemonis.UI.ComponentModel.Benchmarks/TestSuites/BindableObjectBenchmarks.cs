﻿using Anemonis.UI.ComponentModel.Benchmarks.TestObjects;
using BenchmarkDotNet.Attributes;

namespace Anemonis.UI.ComponentModel.Benchmarks.TestSuites
{
    public sealed class BindableObjectBenchmarks
    {
        private readonly TestBindableObject<bool> _bindableObject1 = new TestBindableObject<bool>(false);
        private readonly TestBindableObject<bool> _bindableObject2 = new TestBindableObject<bool>(new TestTargetObject<bool>(false));

        [Benchmark(Description = "get-TYPE=F")]
        public bool GetByField()
        {
            return _bindableObject1.BindableFieldValue;
        }

        [Benchmark(Description = "get-TYPE=P")]
        public bool GetByProperty()
        {
            return _bindableObject2.BindablePropertyValue;
        }

        [Benchmark(Description = "set-TYPE=F")]
        public void SetByField()
        {
            _bindableObject1.BindableFieldValue = _bindableObject1.FieldValue;
        }

        [Benchmark(Description = "get-TYPE=P")]
        public void SetByProperty()
        {
            _bindableObject2.BindablePropertyValue = _bindableObject2.PropertyValue;
        }
    }
}