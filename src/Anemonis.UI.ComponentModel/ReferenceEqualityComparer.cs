// © Alexander Kozlenko. Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Anemonis.UI.ComponentModel
{
    internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly IEqualityComparer<object> Instance = new ReferenceEqualityComparer();

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return object.ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
