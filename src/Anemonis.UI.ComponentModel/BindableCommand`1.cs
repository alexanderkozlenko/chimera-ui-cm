// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable command component.</summary>
    /// <typeparam name="T">The type of the parameter for action and predicate.</typeparam>
    public class BindableCommand<T> : IBindableCommand
    {
        private readonly Action<T> _actionMethod;
        private readonly Predicate<T> _predicateMethod;
        private readonly SynchronizationContext _synchronizationContext;
        private readonly object _observablesLockRoot = new object();

        private IDictionary<INotifyPropertyChanged, string[]> _observables;

        /// <summary>Initializes a new instance of the <see cref="BindableCommand{T}" /> class.</summary>
        /// <param name="actionMethod">The method to execute when the command is executed.</param>
        /// <param name="synchronizationContext">The synchronization context for interaction with UI.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionMethod" /> is <see langword="null" />.</exception>
        public BindableCommand(Action<T> actionMethod, SynchronizationContext synchronizationContext = null)
        {
            if (actionMethod == null)
            {
                throw new ArgumentNullException(nameof(actionMethod));
            }

            _actionMethod = actionMethod;
            _synchronizationContext = synchronizationContext;
        }

        /// <summary>Initializes a new instance of the <see cref="BindableCommand{T}" /> class.</summary>
        /// <param name="actionMethod">The method to execute when the command is executed.</param>
        /// <param name="predicateMethod">The method which is used as a predicate to check if the command can be executed.</param>
        /// <param name="synchronizationContext">The synchronization context for interaction with UI.</param>
        /// <exception cref="ArgumentNullException"><paramref name="actionMethod" /> or <paramref name="predicateMethod" /> is <see langword="null" />.</exception>
        public BindableCommand(Action<T> actionMethod, Predicate<T> predicateMethod, SynchronizationContext synchronizationContext = null)
            : this(actionMethod, synchronizationContext)
        {
            if (predicateMethod == null)
            {
                throw new ArgumentNullException(nameof(predicateMethod));
            }

            _predicateMethod = predicateMethod;
        }

        private void OnObservingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handleEvent = false;

            lock (_observablesLockRoot)
            {
                if (!_observables.TryGetValue(sender as INotifyPropertyChanged, out var propertyNames))
                {
                    return;
                }
                if (e.PropertyName == null)
                {
                    return;
                }

                handleEvent = propertyNames == null;

                if (!handleEvent)
                {
                    var propertyName = e.PropertyName;

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

        bool ICommand.CanExecute(object parameter)
        {
            return (_predicateMethod == null) || (_predicateMethod.Invoke((T)parameter));
        }

        void ICommand.Execute(object parameter)
        {
            _actionMethod.Invoke((T)parameter);
        }

        /// <summary>Raises an event that the command should be required for its state.</summary>
        public virtual void RaiseCanExecuteChanged()
        {
            var synchronizationContext = _synchronizationContext;

            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                synchronizationContext.Post(state => CanExecuteChanged?.Invoke(this, EventArgs.Empty), null);
            }
        }

        /// <summary>Releases all subscriptions to the command state changed event and to the property changed event of the currently observing objects.</summary>
        public virtual void Dispose()
        {
            lock (_observablesLockRoot)
            {
                if (_observables != null)
                {
                    foreach (var observable in _observables.Keys)
                    {
                        observable.PropertyChanged -= OnObservingPropertyChanged;
                    }

                    _observables = null;
                }
            }

            CanExecuteChanged = null;
        }

        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> is <see langword="null" />.</exception>
        public void SubscribePropertyChanged(INotifyPropertyChanged observable)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            lock (_observablesLockRoot)
            {
                if (_observables == null)
                {
                    _observables = new Dictionary<INotifyPropertyChanged, string[]>(ReferenceEqualityComparer<INotifyPropertyChanged>.Instance);
                }

                _observables[observable] = null;

                observable.PropertyChanged += OnObservingPropertyChanged;
            }
        }

        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <param name="propertyNames">The list of property names to handle.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> or <paramref name="propertyNames" /> is <see langword="null" />.</exception>
        public void SubscribePropertyChanged(INotifyPropertyChanged observable, params string[] propertyNames)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }
            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            lock (_observablesLockRoot)
            {
                if (_observables == null)
                {
                    _observables = new Dictionary<INotifyPropertyChanged, string[]>(ReferenceEqualityComparer<INotifyPropertyChanged>.Instance);
                }

                _observables[observable] = propertyNames;

                observable.PropertyChanged += OnObservingPropertyChanged;
            }
        }

        /// <summary>Unsubscribes from property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observable" /> is <see langword="null" />.</exception>
        public void UnsubscribePropertyChanged(INotifyPropertyChanged observable)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            lock (_observablesLockRoot)
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

        /// <summary>Gets or sets the synchronization context for interaction with UI.</summary>
        public SynchronizationContext SynchronizationContext
        {
            get => _synchronizationContext;
        }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public virtual event EventHandler CanExecuteChanged;
    }
}