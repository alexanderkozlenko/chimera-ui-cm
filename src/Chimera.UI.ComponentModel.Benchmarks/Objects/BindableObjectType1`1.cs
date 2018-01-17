﻿namespace Chimera.UI.ComponentModel.Benchmarks.Objects
{
    internal sealed class BindableObjectType1<T> : BindableObject
    {
        public T Source;

        public T Value
        {
            get => GetValue(ref Source);
            set => SetValue(ref Source, value);
        }
    }
}