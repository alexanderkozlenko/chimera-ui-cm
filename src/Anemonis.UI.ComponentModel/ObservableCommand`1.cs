// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable command component with advanced composition abilities.</summary>
    /// <typeparam name="T">The type of the parameter for action and predicate.</typeparam>
    public class ObservableCommand<T> : BindableCommand<T>, IObservableCommand
    {
        private readonly object _syncRoot = new();

        private HashSet<IObserver<EventArgs>> _observers;
        private Dictionary<INotifyPropertyChanged, string[]> _observables;

        /// <summary>Initializes a new instance of the <see cref="ObservableCommand{T}" /> class.</summary>
        /// <param name="actionMethod">The method to execute when the command is executed.</param>
        /// <param name="synchronizationContext">The synchronization context for interaction with UI.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionMethod" /> is <see langword="null" />.</exception>
        public ObservableCommand(Action<T> actionMethod, SynchronizationContext synchronizationContext = null)
            : base(actionMethod, synchronizationContext)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCommand{T}" /> class.</summary>
        /// <param name="actionMethod">The method to execute when the command is executed.</param>
        /// <param name="predicateMethod">The method which is used as a predicate to check if the command can be executed.</param>
        /// <param name="synchronizationContext">The synchronization context for interaction with UI.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionMethod" /> or <paramref name="predicateMethod" /> is <see langword="null" />.</exception>
        public ObservableCommand(Action<T> actionMethod, Predicate<T> predicateMethod, SynchronizationContext synchronizationContext = null)
            : base(actionMethod, predicateMethod, synchronizationContext)
        {
        }

        private void OnObservingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((sender is not INotifyPropertyChanged observable) || (e is null))
            {
                return;
            }

            var handleEvent = false;

            lock (_syncRoot)
            {
                if (_observables is null)
                {
                    return;
                }
                if (!_observables.TryGetValue(observable, out var propertyNames))
                {
                    return;
                }

                handleEvent = propertyNames is null;

                if (!handleEvent)
                {
                    var propertyName = e.PropertyName;

                    for (var i = 0; i < propertyNames.Length; i++)
                    {
                        if (string.Equals(propertyNames[i], propertyName, StringComparison.Ordinal))
                        {
                            handleEvent = true;

                            break;
                        }
                    }
                }
            }

            if (handleEvent)
            {
                RaiseCanExecuteChanged();
            }
        }

        private protected sealed override EventArgs UnsafeRaiseCanExecuteChanged()
        {
            var eventArgs = base.UnsafeRaiseCanExecuteChanged();

            lock (_syncRoot)
            {
                if (_observers is not null)
                {
                    eventArgs ??= CreateCanExecuteChangedEventArgs();

                    var enumerator = _observers.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.OnNext(eventArgs);
                    }
                }
            }

            return eventArgs;
        }

        internal void Unsubscribe(IObserver<EventArgs> observer)
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
                    if (_observables is not null)
                    {
                        var enumerator = _observables.Keys.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.PropertyChanged -= OnObservingPropertyChanged;
                        }

                        _observables = null;
                    }
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
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> is <see langword="null" />.</exception>
        public void Subscribe(INotifyPropertyChanged observable)
        {
            if (observable is null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            lock (_syncRoot)
            {
                _observables ??= new(ReferenceEqualityComparer.Instance);
                _observables[observable] = null;

                observable.PropertyChanged += OnObservingPropertyChanged;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> or <paramref name="propertyNames" /> is <see langword="null" />.</exception>
        public void Subscribe(INotifyPropertyChanged observable, params string[] propertyNames)
        {
            if (observable is null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            if (propertyNames is null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            lock (_syncRoot)
            {
                _observables ??= new(ReferenceEqualityComparer.Instance);
                _observables[observable] = propertyNames;

                observable.PropertyChanged += OnObservingPropertyChanged;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="observer" /> is <see langword="null" />.</exception>
        public IDisposable Subscribe(IObserver<EventArgs> observer)
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

            return new ObservableSubscribeToken<EventArgs>(observer, Unsubscribe);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> is <see langword="null" />.</exception>
        public void Unsubscribe(INotifyPropertyChanged observable)
        {
            if (observable is null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            lock (_syncRoot)
            {
                if (_observables is not null)
                {
                    observable.PropertyChanged -= OnObservingPropertyChanged;

                    _observables.Remove(observable);

                    if (_observables.Count == 0)
                    {
                        _observables = null;
                    }
                }
            }
        }
    }
}
