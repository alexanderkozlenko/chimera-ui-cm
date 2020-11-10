// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using Anemonis.UI.ComponentModel.Resources;

namespace Anemonis.UI.ComponentModel
{
    internal static class PropertyAccessorsCache<TTarget>
    {
        private static readonly object s_syncRoot = new();
        private static readonly Dictionary<string, (object GetAccessor, object SetAccessor)> s_propertyAccessorCache = new();

        private static PropertyInfo FindPropertyInfo(string propertyName)
        {
            var type = typeof(TTarget);

            while (true)
            {
                var propertyInfo = type.GetProperty(propertyName);

                if (propertyInfo is not null)
                {
                    return propertyInfo;
                }
                else if (type.BaseType is not null)
                {
                    type = type.BaseType;
                }
                else
                {
                    return null;
                }
            }
        }

        public static (Func<TTarget, TValue> GetAccessor, Action<TTarget, TValue> SetAccessor) GetPropertyAccessors<TValue>(string propertyName)
        {
            lock (s_syncRoot)
            {
                var propertyGetAccessor = default(object);
                var propertySetAccessor = default(object);

                if (s_propertyAccessorCache.TryGetValue(propertyName, out var propertyAccessors))
                {
                    (propertyGetAccessor, propertySetAccessor) = propertyAccessors;
                }
                else
                {
                    var propertyInfo = FindPropertyInfo(propertyName);

                    if (propertyInfo is null)
                    {
                        throw new MissingMemberException(string.Format(CultureInfo.CurrentCulture, Strings.GetString("property_accessor_cache.property_not_found"), typeof(TTarget), propertyName));
                    }

                    if (propertyInfo.CanRead)
                    {
                        propertyGetAccessor = propertyInfo.GetMethod.CreateDelegate(typeof(Func<TTarget, TValue>));
                    }
                    if (propertyInfo.CanWrite)
                    {
                        propertySetAccessor = propertyInfo.SetMethod.CreateDelegate(typeof(Action<TTarget, TValue>));
                    }

                    s_propertyAccessorCache.Add(propertyName, (propertyGetAccessor, propertySetAccessor));
                }

                return ((Func<TTarget, TValue>)propertyGetAccessor, (Action<TTarget, TValue>)propertySetAccessor);
            }
        }
    }
}
