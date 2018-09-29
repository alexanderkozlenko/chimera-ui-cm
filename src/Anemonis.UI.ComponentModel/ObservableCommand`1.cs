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
        private readonly object _syncRoot = new object();

        private ISet<IObserver<EventArgs>> _observers;
        private IDictionary<INotifyPropertyChanged, string[]> _observables;

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
            var observable = sender as INotifyPropertyChanged;

            if (observable == null)
            {
                return;
            }

            var handleEvent = false;

            lock (_syncRoot)
            {
                if (!_observables.TryGetValue(observable, out var propertyNames))
                {
                    return;
                }

                handleEvent = propertyNames == null;

                if (!handleEvent)
                {
                    var propertyName = e?.PropertyName;

                    for (var i = 0; i < propertyNames.Length; i++)
                    {
                        if (string.Equals(propertyNames[i], propertyName))
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

        private void UnsafeRaiseCanExecuteChanged(IObserver<EventArgs>[] observers, EventArgs args)
        {
            for (var i = 0; i < observers.Length; i++)
            {
                observers[i].OnNext(args);
            }
        }

        private protected sealed override void UnsafeRaiseCanExecuteChanged(SynchronizationContext synchronizationContext)
        {
            base.UnsafeRaiseCanExecuteChanged(synchronizationContext);

            var observerArray = default(IObserver<EventArgs>[]);

            lock (_syncRoot)
            {
                if (_observers == null)
                {
                    return;
                }

                observerArray = new IObserver<EventArgs>[_observers.Count];
                _observers.CopyTo(observerArray, 0);
            }

            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                UnsafeRaiseCanExecuteChanged(observerArray, EventArgs.Empty);
            }
            else
            {
                synchronizationContext.Post(state => UnsafeRaiseCanExecuteChanged(observerArray, EventArgs.Empty), null);
            }
        }

        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> is <see langword="null" />.</exception>
        public void Subscribe(INotifyPropertyChanged observable)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            lock (_syncRoot)
            {
                if (_observables == null)
                {
                    _observables = new Dictionary<INotifyPropertyChanged, string[]>(ReferenceEqualityComparer.Instance);
                }

                _observables[observable] = null;
                observable.PropertyChanged += OnObservingPropertyChanged;
            }
        }

        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <param name="propertyNames">The list of property names to handle.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> or <paramref name="propertyNames" /> is <see langword="null" />.</exception>
        public void Subscribe(INotifyPropertyChanged observable, params string[] propertyNames)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            lock (_syncRoot)
            {
                if (_observables == null)
                {
                    _observables = new Dictionary<INotifyPropertyChanged, string[]>(ReferenceEqualityComparer.Instance);
                }

                _observables[observable] = propertyNames;
                observable.PropertyChanged += OnObservingPropertyChanged;
            }
        }

        /// <summary>Unsubscribes from property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> is <see langword="null" />.</exception>
        public void Unsubscribe(INotifyPropertyChanged observable)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            lock (_syncRoot)
            {
                if (_observables == null)
                {
                    return;
                }

                observable.PropertyChanged -= OnObservingPropertyChanged;
                _observables.Remove(observable);

                if (_observables.Count == 0)
                {
                    _observables = null;
                }
            }
        }

        /// <summary>Notifies the current instance that an observer is to receive notifications about command state changed.</summary>
        /// <param name="observer">The object that is to receive notifications about command state changed.</param>
        /// <returns>A reference to an interface that allows observers to stop receiving notifications about command state changed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observer" /> is <see langword="null" />.</exception>
        public IDisposable Subscribe(IObserver<EventArgs> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            lock (_syncRoot)
            {
                if (_observers == null)
                {
                    _observers = new HashSet<IObserver<EventArgs>>(ReferenceEqualityComparer.Instance);
                }

                _observers.Add(observer);
            }

            return new ObservableCommandSubscribeToken<T>(this, observer);
        }

        internal void Unsubscribe(IObserver<EventArgs> observer)
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

        /// <summary>Releases all subscriptions to the command state changed event and to the property changed event of the currently observing objects.</summary>
        public override void Dispose()
        {
            lock (_syncRoot)
            {
                if (_observables != null)
                {
                    foreach (var observable in _observables.Keys)
                    {
                        observable.PropertyChanged -= OnObservingPropertyChanged;
                    }

                    _observables = null;
                }
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