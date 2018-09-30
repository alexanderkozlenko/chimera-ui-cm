// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Anemonis.UI.ComponentModel.Resources;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable object component.</summary>
    public abstract class BindableObject : IBindableObject
    {
        private SynchronizationContext _synchronizationContext;

        /// <summary>Initializes a new instance of the <see cref="BindableObject" /> class.</summary>
        protected BindableObject()
        {
        }

        private void UnsafeRaisePropertyChanged(string propertyName, SynchronizationContext synchronizationContext)
        {
            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                UnsafeRaisePropertyChanged(propertyName);
            }
            else
            {
                synchronizationContext.Post(state => UnsafeRaisePropertyChanged(propertyName), null);
            }
        }

        private protected virtual void UnsafeRaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Raises the event about a changed property.</summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            UnsafeRaisePropertyChanged(propertyName, _synchronizationContext);
        }

        /// <summary>Gets a value from the field.</summary>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <param name="field">The field to get a value from.</param>
        /// <returns>The value of the field.</returns>
        protected TValue GetValue<TValue>(ref TValue field)
        {
            return field;
        }

        /// <summary>Gets a value from the object's property.</summary>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="target">The object that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="defaultValue">The value to return if the object is <see langword="null" />.</param>
        /// <returns>The value of the object's property.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property does not have the get accessor.</exception>
        /// <exception cref="MissingMemberException">The specified property is not found.</exception>
        protected TValue GetValue<TTarget, TValue>(TTarget target, string propertyName, TValue defaultValue)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (target == null)
            {
                return defaultValue;
            }

            var propertyInfo = PropertyInfoCache<TTarget>.GetPropertyInfo(propertyName);

            if (!propertyInfo.CanRead)
            {
                throw new InvalidOperationException(string.Format(Strings.GetString("bindable_object.property_info.no_get_accessor"), propertyName));
            }

            return (TValue)propertyInfo.GetValue(target);
        }

        /// <summary>Sets the value to the field if it differs and notify listeners.</summary>
        /// <typeparam name="TValue">The type of the field.</typeparam>
        /// <param name="field">The field to set value to.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="callback">The method to execute if the value has been changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. The value is provided by the runtime.</param>
        /// <exception cref="ArgumentNullException"><paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        protected void SetValue<TValue>(ref TValue field, TValue value, Action callback = null, [CallerMemberName] string outerPropertyName = null)
        {
            if (outerPropertyName == null)
            {
                throw new ArgumentNullException(nameof(outerPropertyName));
            }
            if (EqualityComparer<TValue>.Default.Equals(value, field))
            {
                return;
            }

            field = value;

            UnsafeRaisePropertyChanged(outerPropertyName, _synchronizationContext);

            callback?.Invoke();
        }

        /// <summary>Sets the value to the object's property if it differs and notify listeners.</summary>
        /// <typeparam name="TTarget">The type of the target object.</typeparam>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="target">The object that declares the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The desired value to set.</param>
        /// <param name="callback">The method to execute if the value has been changed.</param>
        /// <param name="outerPropertyName">The name of the property to raise change notification for. The value is provided by the runtime.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> or <paramref name="outerPropertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified property does not have the get or set accessor.</exception>
        /// <exception cref="MissingMemberException">The specified property is not found.</exception>
        protected void SetValue<TTarget, TValue>(TTarget target, string propertyName, TValue value, Action callback = null, [CallerMemberName] string outerPropertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (outerPropertyName == null)
            {
                throw new ArgumentNullException(nameof(outerPropertyName));
            }
            if (target == null)
            {
                return;
            }

            var propertyInfo = PropertyInfoCache<TTarget>.GetPropertyInfo(propertyName);

            if (!propertyInfo.CanRead)
            {
                throw new InvalidOperationException(string.Format(Strings.GetString("bindable_object.property_info.no_get_accessor"), propertyName));
            }
            if (!propertyInfo.CanWrite)
            {
                throw new InvalidOperationException(string.Format(Strings.GetString("bindable_object.property_info.no_set_accessor"), propertyName));
            }

            var propertyValue = (TValue)propertyInfo.GetValue(target);

            if (EqualityComparer<TValue>.Default.Equals(value, propertyValue))
            {
                return;
            }

            propertyInfo.SetValue(target, value);

            UnsafeRaisePropertyChanged(outerPropertyName, _synchronizationContext);

            callback?.Invoke();
        }

        /// <summary>Subscribes the current instance to the required notifications.</summary>
        public virtual void Subscribe()
        {
        }

        /// <summary>Unsubscribes the current instance from the required notifications.</summary>
        public virtual void Unsubscribe()
        {
        }

        /// <summary>Releases all subscriptions to the property changed event.</summary>
        public virtual void Dispose()
        {
            PropertyChanged = null;
        }

        /// <summary>Gets or sets the synchronization context for interaction with UI.</summary>
        public SynchronizationContext SynchronizationContext
        {
            get => _synchronizationContext;
            set => _synchronizationContext = value;
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}