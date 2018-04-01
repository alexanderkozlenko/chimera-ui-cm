using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using Chimera.UI.ComponentModel.Resources;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Represents a bindable command component.</summary>
    public class BindableCommand : IBindableCommand
    {
        private Action<object> _commandAction;
        private Predicate<object> _commandPredicate;
        private INotifyPropertyChanged _observingObject;
        private ISet<string> _observingProperties;

        /// <summary>Initializes a new instance of the <see cref="BindableCommand" /> class.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="predicate">The predicate to check if the command can be executed.</param>
        /// <param name="observingObject">The <see cref="INotifyPropertyChanged" /> as a source of observing properties.</param>
        /// <param name="synchronizationContext">The synchronization context to interact with UI through.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" />.</exception>
        public BindableCommand(Action<object> action, Predicate<object> predicate = null, INotifyPropertyChanged observingObject = null, SynchronizationContext synchronizationContext = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _commandAction = action;
            _commandPredicate = predicate;
            _observingObject = observingObject;

            if (observingObject != null)
            {
                _observingProperties = new HashSet<string>(StringComparer.Ordinal);

                observingObject.PropertyChanged += PropertyChangedEventHandler;
            }

            SynchronizationContext = synchronizationContext ?? SynchronizationContext.Current;
        }

        /// <summary>Starts observing a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyNames">The names of the properties.</param>
        /// <exception cref="InvalidOperationException">The source of observing properties is <see langword="null" />.</exception>
        public void StartObservingProperties(params string[] propertyNames)
        {
            if (_observingObject == null)
            {
                throw new InvalidOperationException(Strings.GetString("command.observing_object.null"));
            }

            for (var i = 0; i < propertyNames.Length; i++)
            {
                _observingProperties.Add(propertyNames[i]);
            }
        }

        /// <summary>Stops observing a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyNames">The names of the properties.</param>
        /// <exception cref="InvalidOperationException">The source of observing properties is <see langword="null" />.</exception>
        public void StopObservingProperties(params string[] propertyNames)
        {
            if (_observingObject == null)
            {
                throw new InvalidOperationException(Strings.GetString("command.observing_object.null"));
            }

            for (var i = 0; i < propertyNames.Length; i++)
            {
                _observingProperties.Remove(propertyNames[i]);
            }
        }

        /// <summary>Raises an event that the command should be required for its state.</summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return _commandPredicate?.Invoke(parameter) != false;
        }

        void ICommand.Execute(object parameter)
        {
            _commandAction?.Invoke(parameter);
        }

        /// <summary>Releases all references and subscriptions of the command.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Executes raising an event that the command should be required for its state.</summary>
        protected virtual void OnCanExecuteChanged()
        {
            var synchronizationContext = SynchronizationContext;

            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                synchronizationContext.Post(CanExecuteChangedContextAction, null);
            }
        }

        /// <summary>Releases all references and subscriptions of the command.</summary>
        /// <param name="disposing">Indicates whether the method was not invoked by finalyzer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            var observingObject = _observingObject;

            if (observingObject != null)
            {
                observingObject.PropertyChanged -= PropertyChangedEventHandler;
            }

            _observingObject = null;
            _observingProperties = null;
            _commandPredicate = null;
            _commandAction = null;

            CanExecuteChanged = null;
        }

        private void CanExecuteChangedContextAction(object state)
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_observingProperties?.Contains(e.PropertyName) == true)
            {
                OnCanExecuteChanged();
            }
        }

        /// <summary>Gets the synchronization context to interact with UI through.</summary>
        public SynchronizationContext SynchronizationContext
        {
            get;
        }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged;
    }
}