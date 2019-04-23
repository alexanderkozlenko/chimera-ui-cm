// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;

using Anemonis.UI.ComponentModel.Resources;

namespace Anemonis.UI.ComponentModel
{
    internal static class PropertyAccessorsCache<TTarget>
    {
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<string, (object GetAccessor, object SetAccessor)> _propertyAccessorCache = new Dictionary<string, (object, object)>();

        private static PropertyInfo FindPropertyInfo(string propertyName)
        {
            var propertyInfo = default(PropertyInfo);
            var typeInfo = typeof(TTarget).GetTypeInfo();

            while ((propertyInfo == null) && (typeInfo != null))
            {
                propertyInfo = typeInfo.GetDeclaredProperty(propertyName);

                if (propertyInfo == null)
                {
                    typeInfo = typeInfo.BaseType?.GetTypeInfo();
                }
            }

            return propertyInfo;
        }

        public static (Func<TTarget, TValue> GetAccessor, Action<TTarget, TValue> SetAccessor) GetPropertyAccessors<TValue>(string propertyName)
        {
            lock (_syncRoot)
            {
                var propertyGetAccessor = default(object);
                var propertySetAccessor = default(object);

                if (_propertyAccessorCache.TryGetValue(propertyName, out var propertyAccessors))
                {
                    (propertyGetAccessor, propertySetAccessor) = propertyAccessors;
                }
                else
                {
                    var propertyInfo = FindPropertyInfo(propertyName);

                    if (propertyInfo == null)
                    {
                        throw new MissingMemberException(string.Format(Strings.GetString("property_accessor_cache.property_not_found"), typeof(TTarget), propertyName));
                    }

                    if (propertyInfo.CanRead)
                    {
                        propertyGetAccessor = propertyInfo.GetMethod.CreateDelegate(typeof(Func<TTarget, TValue>));
                    }
                    if (propertyInfo.CanWrite)
                    {
                        propertySetAccessor = propertyInfo.SetMethod.CreateDelegate(typeof(Action<TTarget, TValue>));
                    }

                    _propertyAccessorCache.Add(propertyName, (propertyGetAccessor, propertySetAccessor));
                }

                return ((Func<TTarget, TValue>)propertyGetAccessor, (Action<TTarget, TValue>)propertySetAccessor);
            }
        }
    }
}
