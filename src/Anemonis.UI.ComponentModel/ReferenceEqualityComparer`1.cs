// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
         where T : class
    {
        private static readonly ReferenceEqualityComparer<T> _instance = new ReferenceEqualityComparer<T>();

        public bool Equals(T x, T y)
        {
            return object.ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public static ReferenceEqualityComparer<T> Instance
        {
            get => _instance;
        }
    }
}