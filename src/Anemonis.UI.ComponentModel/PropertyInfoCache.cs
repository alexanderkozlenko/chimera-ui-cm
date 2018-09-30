// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using Anemonis.UI.ComponentModel.Resources;

namespace Anemonis.UI.ComponentModel
{
    internal static class PropertyInfoCache
    {
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<PropertyInfoKey, PropertyInfo> _propertyInfoCache = new Dictionary<PropertyInfoKey, PropertyInfo>();

        private static PropertyInfo FindPropertyInfo(in PropertyInfoKey propertyInfoKey)
        {
            var propertyInfo = default(PropertyInfo);
            var typeInfo = propertyInfoKey.DeclaringType.GetTypeInfo();

            while ((propertyInfo == null) && (typeInfo != null))
            {
                propertyInfo = typeInfo.GetDeclaredProperty(propertyInfoKey.PropertyName);

                if (propertyInfo == null)
                {
                    typeInfo = typeInfo.BaseType?.GetTypeInfo();
                }
            }

            return propertyInfo;
        }

        public static PropertyInfo GetPropertyInfo(in PropertyInfoKey propertyInfoKey)
        {
            lock (_syncRoot)
            {
                if (!_propertyInfoCache.TryGetValue(propertyInfoKey, out var propertyInfo))
                {
                    propertyInfo = FindPropertyInfo(propertyInfoKey);

                    if (propertyInfo == null)
                    {
                        throw new MissingMemberException(string.Format(Strings.GetString("property_info_cache.property_not_found"), propertyInfoKey.DeclaringType, propertyInfoKey.PropertyName));
                    }

                    _propertyInfoCache.Add(propertyInfoKey, propertyInfo);
                }

                return propertyInfo;
            }
        }
    }
}