using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Represents a bindable object component.</summary>
    public abstract class BindableObject : IBindableObject
    {
        private ConcurrentDictionary<PropertyInfoKey, PropertyInfo> _propertyDefinitions;

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

            _propertyDefinitions?.Clear();
            _propertyDefinitions = null;

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

            if ((SynchronizationContext == null) || (SynchronizationContext == SynchronizationContext.Current))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                SynchronizationContext.Post(state => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
            }
        }

        /// <summary>Gets a value from the field.</summary>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <param name="storage">The field to get a value from.</param>
        /// <returns>The value of the field.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <exception cref="InvalidOperationException">The specified property is not found.</exception>
        protected TValue GetValue<TStorage, TValue>(TStorage storageObject, string propertyName, TValue defaultValue)
        {
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
        /// <param name="outerPropertyName">The name of the property to raise change notification for. This value is provided automatically.</param>
        /// <exception cref="ArgumentNullException"><paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        protected void SetValue<TValue>(ref TValue storage, TValue value, [CallerMemberName] string outerPropertyName = null)
        {
            if (outerPropertyName == null)
            {
                throw new ArgumentNullException(nameof(outerPropertyName));
            }

            SetValueInternal(ref storage, value, null, outerPropertyName);
        }

        /// <summary>Sets the value to the field if it differs and notify listeners.</summary>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <param name="storage">The field to set value to.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="action">The action to execute in case the value was changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. This value is provided automatically.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> or <paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        protected void SetValue<TValue>(ref TValue storage, TValue value, Action action, [CallerMemberName] string outerPropertyName = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
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
        /// <param name="outerPropertyName">The name of the property to raise change notification for. This value is provided automatically.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> or <paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property is not found.</exception>
        protected void SetValue<TStorage, TValue>(TStorage storageObject, string propertyName, TValue value, [CallerMemberName] string outerPropertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (outerPropertyName == null)
            {
                throw new ArgumentNullException(nameof(outerPropertyName));
            }

            SetValueInternal(storageObject, propertyName, value, null, outerPropertyName);
        }

        /// <summary>Sets the value to the object's property if it differs and notify listeners.</summary>
        /// <typeparam name="TStorage">The type of the object.</typeparam>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="storageObject">The object that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="action">The action to execute in case the value was changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. This value is provided automatically.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" />, <paramref name="action" />, or <paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property is not found.</exception>
        protected void SetValue<TStorage, TValue>(TStorage storageObject, string propertyName, TValue value, Action action, [CallerMemberName] string outerPropertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private PropertyInfo GetPropertyInfo<TStorage>(string propertyName)
        {
            LazyInitializer.EnsureInitialized(ref _propertyDefinitions, () => new ConcurrentDictionary<PropertyInfoKey, PropertyInfo>());

            return _propertyDefinitions.GetOrAdd(new PropertyInfoKey(typeof(TStorage), propertyName), key => GetPropertyInfo(typeof(TStorage), propertyName));
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            var result = default(PropertyInfo);
            var typeInfo = type.GetTypeInfo();

            while ((result == null) && (typeInfo != null))
            {
                var propertyInfo = typeInfo.GetDeclaredProperty(propertyName);

                if ((propertyInfo != null) && !propertyInfo.GetMethod.IsStatic && !propertyInfo.SetMethod.IsStatic)
                {
                    result = propertyInfo;
                }
                else
                {
                    typeInfo = typeInfo.BaseType?.GetTypeInfo();
                }
            }

            return result ?? throw new InvalidOperationException($"The instance property \"{propertyName}\" was not found on the type \"{type}\" or any underlying type");
        }

        /// <summary>Gets the synchronization context to interact with UI through.</summary>
        public SynchronizationContext SynchronizationContext { get; set; }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private struct PropertyInfoKey
        {
            private readonly Type _declaringType;
            private readonly string _propertyName;

            public PropertyInfoKey(Type declaringType, string propertyName)
            {
                _declaringType = declaringType;
                _propertyName = propertyName;
            }

            public override int GetHashCode()
            {
                return _declaringType.GetHashCode() ^ _propertyName.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var objB = (PropertyInfoKey)obj;

                return object.Equals(objB._declaringType, _declaringType) && object.Equals(objB._propertyName, _propertyName);
            }
        }
    }
}