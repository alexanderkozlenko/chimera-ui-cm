// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using Anemonis.UI.ComponentModel.Resources;

namespace Anemonis.UI.ComponentModel
{
    internal static class PropertyInfoCache<T>
    {
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<string, PropertyInfo> _propertyInfoCache = new Dictionary<string, PropertyInfo>();

        private static PropertyInfo FindPropertyInfo(string propertyName)
        {
            var propertyInfo = default(PropertyInfo);
            var typeInfo = typeof(T).GetTypeInfo();

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

        public static PropertyInfo GetPropertyInfo(string propertyName)
        {
            lock (_syncRoot)
            {
                if (!_propertyInfoCache.TryGetValue(propertyName, out var propertyInfo))
                {
                    propertyInfo = FindPropertyInfo(propertyName);

                    if (propertyInfo == null)
                    {
                        throw new MissingMemberException(string.Format(Strings.GetString("property_info_cache.property_not_found"), typeof(T), propertyName));
                    }

                    _propertyInfoCache.Add(propertyName, propertyInfo);
                }

                return propertyInfo;
            }
        }
    }
}