// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable object component with advanced composition abilities.</summary>
    public abstract class ObservableObject : BindableObject, IObservableObject
    {
        private readonly object _syncRoot = new object();

        private HashSet<IObserver<PropertyChangedEventArgs>> _observers;

        /// <summary>Initializes a new instance of the <see cref="ObservableObject" /> class.</summary>
        protected ObservableObject()
        {
        }

        private protected sealed override PropertyChangedEventArgs UnsafeRaisePropertyChanged(string propertyName)
        {
            var eventArgs = base.UnsafeRaisePropertyChanged(propertyName);

            lock (_syncRoot)
            {
                if (_observers != null)
                {
                    if (eventArgs == null)
                    {
                        eventArgs = CreatePropertyChangedEventArgs(propertyName);
                    }

                    var enumerator = _observers.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.OnNext(eventArgs);
                    }
                }
            }

            return eventArgs;
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

            return new ObservableSubscribeToken<PropertyChangedEventArgs>(observer, Unsubscribe);
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
                    var enumerator = _observers.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.OnCompleted();
                    }

                    _observers = null;
                }
            }

            base.Dispose();
        }
    }
}
