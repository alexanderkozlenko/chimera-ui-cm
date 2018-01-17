using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Chimera.UI.ComponentModel.Resources;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Represents a bindable object component.</summary>
    public abstract class BindableObject : IBindableObject
    {
        private ConcurrentDictionary<PropertyInfoKey, PropertyInfo> _properties;

        /// <summary>Initializes a new instance of the <see cref="BindableObject" /> class.</summary>
        protected BindableObject()
        {
        }

        /// <summary>Releases all references of the bindable object.</summary>
        public void Dispose()
        {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases all references and subscriptions of the command.</summary>
        /// <param name="disposing">Indicates whether the method was not invoked by finalyzer.</param>
        protected virtual void OnDispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _properties?.Clear();
            _properties = null;

            SynchronizationContext = null;

            var subscribers = PropertyChanged?.GetInvocationList();

            if (subscribers != null)
            {
                for (var i = 0; i < subscribers.Length; i++)
                {
                    PropertyChanged -= (PropertyChangedEventHandler)subscribers[i];
                }
            }
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

            var synchronizationContext = SynchronizationContext;

            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                synchronizationContext.Post(state => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
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
        /// <param name="defaultValue">The value to return if the object is uninitialized.</param>
        /// <returns>The value of the object's property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property is not found.</exception>
        protected TValue GetValue<TStorage, TValue>(TStorage storageObject, string propertyName, TValue defaultValue)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (object.Equals(storageObject, default(TStorage)))
            {
                return defaultValue;
            }

            var propertyInfo = GetPropertyInfo<TStorage>(propertyName);

            return (TValue)propertyInfo.GetValue(storageObject);
        }

        /// <summary>Sets the value to the field if it differs and notify listeners.</summary>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <param name="storage">The field to set value to.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="action">The action to execute in case the value was changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. This value is provided automatically.</param>
        /// <exception cref="ArgumentNullException"><paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        protected void SetValue<TValue>(ref TValue storage, TValue value, Action action = null, [CallerMemberName] string outerPropertyName = null)
        {
            if (outerPropertyName == null)
            {
                throw new ArgumentNullException(nameof(outerPropertyName));
            }

            SetValueInternal(ref storage, value, action, outerPropertyName);
        }

        /// <summary>Sets the value to the object's property if it differs and notify listeners.</summary>
        /// <typeparam name="TStorage">The type of the object.</typeparam>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="storageObject">The object that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="action">The action to execute in case the value was changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. This value is provided automatically.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> or <paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property is not found.</exception>
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

            SetValueInternal(storageObject, propertyName, value, action, outerPropertyName);
        }

        private void SetValueInternal<TValue>(ref TValue storage, TValue value, Action action, string propertyName)
        {
            if (object.Equals(storage, value))
            {
                return;
            }

            storage = value;

            OnPropertyChanged(propertyName);

            action?.Invoke();
        }

        private void SetValueInternal<TStorage, TValue>(TStorage storageObject, string propertyName, TValue value, Action action, string outerPropertyName)
        {
            if (object.Equals(storageObject, default(TStorage)))
            {
                return;
            }

            var propertyInfo = GetPropertyInfo<TStorage>(propertyName);

            if (object.Equals(value, propertyInfo.GetValue(storageObject)))
            {
                return;
            }

            propertyInfo.SetValue(storageObject, value);

            OnPropertyChanged(outerPropertyName);

            action?.Invoke();
        }

        private PropertyInfo GetPropertyInfo<TStorage>(string propertyName)
        {
            LazyInitializer.EnsureInitialized(ref _properties, CreatePropertiesDictionary);

            return _properties.GetOrAdd(new PropertyInfoKey(typeof(TStorage), propertyName), GetPropertyInfo);
        }

        private static ConcurrentDictionary<PropertyInfoKey, PropertyInfo> CreatePropertiesDictionary()
        {
            return new ConcurrentDictionary<PropertyInfoKey, PropertyInfo>();
        }

        private static PropertyInfo GetPropertyInfo(PropertyInfoKey key)
        {
            var result = default(PropertyInfo);
            var typeInfo = key.Type.GetTypeInfo();

            while ((result == null) && (typeInfo != null))
            {
                var propertyInfo = typeInfo.GetDeclaredProperty(key.Name);

                if ((propertyInfo != null) && !propertyInfo.GetMethod.IsStatic && !propertyInfo.SetMethod.IsStatic)
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
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("object.property.not_found"), key.Name, key.Type));
            }

            return result;
        }

        /// <summary>Gets the synchronization context to interact with UI through.</summary>
        public SynchronizationContext SynchronizationContext
        {
            get;
            set;
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly struct PropertyInfoKey
        {
            private readonly Type _type;
            private readonly string _name;

            public PropertyInfoKey(Type type, string name)
            {
                _type = type;
                _name = name;
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

            public override bool Equals(object obj)
            {
                var objB = (PropertyInfoKey)obj;

                return _type.Equals(objB._type) && (_name == objB._name);
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
}