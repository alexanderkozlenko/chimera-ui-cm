// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable object component with advanced composition abilities.</summary>
    public abstract class ObservableObject : BindableObject, IObservableObject
    {
        private readonly object _syncRoot = new object();

        private ISet<IObserver<PropertyChangedEventArgs>> _observers;

        /// <summary>Initializes a new instance of the <see cref="ObservableObject" /> class.</summary>
        protected ObservableObject()
        {
        }

        private void UnsafeRaisePropertyChanged(IObserver<PropertyChangedEventArgs>[] observers, PropertyChangedEventArgs args)
        {
            for (var i = 0; i < observers.Length; i++)
            {
                observers[i].OnNext(args);
            }
        }

        private protected sealed override void UnsafeRaisePropertyChanged(string propertyName, SynchronizationContext synchronizationContext)
        {
            base.UnsafeRaisePropertyChanged(propertyName, synchronizationContext);

            var observerArray = default(IObserver<PropertyChangedEventArgs>[]);

            lock (_syncRoot)
            {
                var observers = _observers;

                if (observers == null)
                {
                    return;
                }

                observerArray = new IObserver<PropertyChangedEventArgs>[observers.Count];
                observers.CopyTo(observerArray, 0);
            }

            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                UnsafeRaisePropertyChanged(observerArray, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                synchronizationContext.Post(state => UnsafeRaisePropertyChanged(observerArray, new PropertyChangedEventArgs(propertyName)), null);
            }
        }

        /// <summary>Notifies the current instance that an observer is to receive notifications about property changed.</summary>
        /// <param name="observer">The object that is to receive notifications about property changed.</param>
        /// <returns>A reference to an interface that allows observers to stop receiving notifications about property changed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observer" /> is <see langword="null" />.</exception>
        public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (_syncRoot)
            {
                if (_observers == null)
                {
                    _observers = new HashSet<IObserver<PropertyChangedEventArgs>>(ReferenceEqualityComparer.Instance);
                }

                _observers.Add(observer);
            }

            return new ObservableObjectSubscribeToken(this, observer);
        }

        internal void Unsubscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            lock (_syncRoot)
            {
                if (_observers != null)
                {
                    _observers.Remove(observer);

                    if (_observers.Count == 0)
                    {
                        _observers = null;
                    }
                }
            }
        }

        /// <summary>Releases all subscriptions to the property changed event.</summary>
        public override void Dispose()
        {
            lock (_syncRoot)
            {
                if (_observers != null)
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnCompleted();
                    }

                    _observers = null;
                }
            }

            base.Dispose();
        }
    }
}