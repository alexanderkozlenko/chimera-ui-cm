// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable object component with advanced composition abilities.</summary>
    public abstract class ObservableObject : BindableObject, IObservableObject
    {
        private readonly object _syncRoot = new();

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
                if (_observers is not null)
                {
                    eventArgs ??= CreatePropertyChangedEventArgs(propertyName);

                    var enumerator = _observers.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.OnNext(eventArgs);
                    }
                }
            }

            return eventArgs;
        }

        internal void Unsubscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            lock (_syncRoot)
            {
                if (_observers is not null)
                {
                    _observers.Remove(observer);

                    if (_observers.Count == 0)
                    {
                        _observers = null;
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                lock (_syncRoot)
                {
                    if (_observers is not null)
                    {
                        var enumerator = _observers.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.OnCompleted();
                        }

                        _observers = null;
                    }
                }
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="observer" /> is <see langword="null" />.</exception>
        public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            if (observer is null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (_syncRoot)
            {
                _observers ??= new(ReferenceEqualityComparer.Instance);
                _observers.Add(observer);
            }

            return new ObservableSubscribeToken<PropertyChangedEventArgs>(observer, Unsubscribe);
        }
    }
}
