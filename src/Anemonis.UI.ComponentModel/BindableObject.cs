// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Anemonis.UI.ComponentModel.Resources;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable object component.</summary>
    public abstract class BindableObject : IBindableObject
    {
        private static readonly ConcurrentDictionary<TypeMemberKey, PropertyInfo> _propertiesCache = new ConcurrentDictionary<TypeMemberKey, PropertyInfo>();

        /// <summary>Initializes a new instance of the <see cref="BindableObject" /> class.</summary>
        protected BindableObject()
        {
            SynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>Releases all subscriptions of the object.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases all subscriptions of the object.</summary>
        /// <param name="disposing">Indicates whether the method was not invoked by finalyzer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            PropertyChanged = null;
        }

        /// <summary>Raises an event about changed property.</summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            OnPropertyChanged(propertyName);
        }

        /// <summary>Executes raising an event about changed property.</summary>
        /// <param name="propertyName">The name of the changed property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            var synchronizationContext = SynchronizationContext;

            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                PropertyChanged?.Invoke(this, eventArgs);
            }
            else
            {
                synchronizationContext.Post(state => PropertyChanged?.Invoke(this, eventArgs), null);
            }
        }

        /// <summary>Gets a value from the field.</summary>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <param name="storage">The field to get a value from.</param>
        /// <returns>The value of the field.</returns>
        protected TValue GetValue<TValue>(ref TValue storage)
        {
            return storage;
        }

        /// <summary>Gets a value from the object's property.</summary>
        /// <typeparam name="TStorage">The type of the object.</typeparam>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="storageObject">The object that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="defaultValue">The value to return if the object is <see langword="null" />.</param>
        /// <returns>The value of the object's property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property is not found, or is static, or does not have get or set accessor.</exception>
        protected TValue GetValue<TStorage, TValue>(TStorage storageObject, string propertyName, TValue defaultValue)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (storageObject == null)
            {
                return defaultValue;
            }

            var propertyInfo = _propertiesCache.GetOrAdd(new TypeMemberKey(typeof(TStorage), propertyName), GetPropertyInfo);

            return (TValue)propertyInfo.GetValue(storageObject);
        }

        /// <summary>Sets the value to the field if it differs and notify listeners.</summary>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <param name="storage">The field to set value to.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="action">The action to execute in case the value was changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. The value is provided by the runtime.</param>
        /// <exception cref="ArgumentNullException"><paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        protected void SetValue<TValue>(ref TValue storage, TValue value, Action action = null, [CallerMemberName] string outerPropertyName = null)
        {
            if (outerPropertyName == null)
            {
                throw new ArgumentNullException(nameof(outerPropertyName));
            }
            if (EqualityComparer<TValue>.Default.Equals(value, storage))
            {
                return;
            }

            storage = value;

            OnPropertyChanged(outerPropertyName);

            action?.Invoke();
        }

        /// <summary>Sets the value to the object's property if it differs and notify listeners.</summary>
        /// <typeparam name="TStorage">The type of the object.</typeparam>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="storageObject">The object that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="action">The action to execute in case the value was changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. The value is provided by the runtime.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> or <paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property is not found, or is static, or does not have get or set accessor.</exception>
        protected void SetValue<TStorage, TValue>(TStorage storageObject, string propertyName, TValue value, Action action = null, [CallerMemberName] string outerPropertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (outerPropertyName == null)
            {
                throw new ArgumentNullException(nameof(outerPropertyName));
            }
            if (storageObject == null)
            {
                return;
            }

            var propertyInfo = _propertiesCache.GetOrAdd(new TypeMemberKey(typeof(TStorage), propertyName), GetPropertyInfo);

            if (EqualityComparer<TValue>.Default.Equals(value, (TValue)propertyInfo.GetValue(storageObject)))
            {
                return;
            }

            propertyInfo.SetValue(storageObject, value);

            OnPropertyChanged(outerPropertyName);

            action?.Invoke();
        }

        private static PropertyInfo GetPropertyInfo(TypeMemberKey key)
        {
            var result = default(PropertyInfo);
            var typeInfo = key.Type.GetTypeInfo();

            while ((result == null) && (typeInfo != null))
            {
                var propertyInfo = typeInfo.GetDeclaredProperty(key.Name);

                if ((propertyInfo != null) && propertyInfo.CanRead && propertyInfo.CanWrite && !propertyInfo.GetMethod.IsStatic)
                {
                    result = propertyInfo;
                }
                else
                {
                    typeInfo = typeInfo.BaseType?.GetTypeInfo();
                }
            }

            if (result == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("object.property.not_found"), key.Type, key.Name));
            }

            return result;
        }

        /// <summary>Gets or sets the synchronization context to interact with UI through.</summary>
        public SynchronizationContext SynchronizationContext
        {
            get;
            set;
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}