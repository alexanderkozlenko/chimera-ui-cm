// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    internal readonly struct PropertyInfoKey : IEquatable<PropertyInfoKey>
    {
        private readonly Type _declaringType;
        private readonly string _propertyName;

        public PropertyInfoKey(Type declaringType, string propertyName)
        {
            _declaringType = declaringType;
            _propertyName = propertyName;
        }

        public bool Equals(PropertyInfoKey other)
        {
            return (_declaringType == other._declaringType) && string.Equals(_propertyName, other._propertyName);
        }

        public override bool Equals(object obj)
        {
            return (obj is PropertyInfoKey other) && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)2166136261;

                if (_declaringType != null)
                {
                    hashCode ^= _declaringType.GetHashCode();
                    hashCode *= 16777619;
                }
                if (_propertyName != null)
                {
                    hashCode ^= _propertyName.GetHashCode();
                    hashCode *= 16777619;
                }

                return hashCode;
            }
        }

        public Type DeclaringType
        {
            get => _declaringType;
        }

        public string PropertyName
        {
            get => _propertyName;
        }
    }
}