// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Chimera.UI.ComponentModel
{
    internal readonly struct TypeMemberKey
    {
        internal readonly Type Type;
        internal readonly string Name;

        public TypeMemberKey(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return (obj is TypeMemberKey other) && Type.Equals(other.Type) && string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)2166136261;

                if (Type != null)
                {
                    hashCode ^= Type.GetHashCode();
                    hashCode *= 16777619;
                }
                if (Name != null)
                {
                    hashCode ^= Name.GetHashCode();
                    hashCode *= 16777619;
                }

                return hashCode;
            }
        }
    }
}