using System;

namespace Chimera.UI.ComponentModel.Internal
{
    internal readonly struct TypeMemberKey
    {
        private readonly Type _type;
        private readonly string _name;

        public TypeMemberKey(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        public override bool Equals(object obj)
        {
            return (obj is TypeMemberKey other) && _type.Equals(other._type) && (_name == other._name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (int)2166136261;

                result = (result * 16777619) ^ _type.GetHashCode();
                result = (result * 16777619) ^ _name.GetHashCode();

                return result;
            }
        }

        public Type Type
        {
            get => _type;
        }

        public string Name
        {
            get => _name;
        }
    }
}