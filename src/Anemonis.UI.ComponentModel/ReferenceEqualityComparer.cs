// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        private static readonly IEqualityComparer<object> _instance = new ReferenceEqualityComparer();

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return object.ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }

        public static IEqualityComparer<object> Instance
        {
            get => _instance;
        }
    }
}