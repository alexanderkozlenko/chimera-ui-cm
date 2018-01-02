using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using Chimera.UI.ComponentModel.Resources;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Represents a bindable command component.</summary>
    public class BindableCommand : IBindableCommand
    {
        private Action<object> _action;
        private Predicate<object> _predicate;
        private IBindableObject _trackingObject;
        private ConcurrentDictionary<string, byte> _trackingProperties;

        /// <summary>Initializes a new instance of the <see cref="BindableCommand" /> class.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="predicate">The predicate to check if the command can be executed.</param>
        /// <param name="trackingObject">The <see cref="IBindableObject" /> as a source of tracking properties and a synchronization context.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" />.</exception>
        public BindableCommand(Action<object> action, Predicate<object> predicate = null, IBindableObject trackingObject = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _action = action;
            _predicate = predicate;
            _trackingObject = trackingObject;

            if (trackingObject != null)
            {
                _trackingProperties = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

                SynchronizationContext = trackingObject.SynchronizationContext;

                trackingObject.PropertyChanged += PropertyChangedEventHandler;
            }
        }

        /// <summary>Starts tracking a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance of <see cref="IBindableCommand" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The source of tracking properties is not defined or invalid.</exception>
        public IBindableCommand StartTrackingProperty(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (_trackingObject == null)
            {
                throw new InvalidOperationException(Strings.GetString("command.object.undefined"));
            }

            _trackingProperties?.TryAdd(propertyName, default);

            return this;
        }

        /// <summary>Stops tracking a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance of <see cref="IBindableCommand" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The source of tracking properties is not defined or invalid.</exception>
        public IBindableCommand StopTrackingProperty(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            if (_trackingObject == null)
            {
                throw new InvalidOperationException(Strings.GetString("command.object.undefined"));
            }

            _trackingProperties?.TryRemove(propertyName, out var _);

            return this;
        }

        /// <summary>Raises an event that the command should be requeried for its state.</summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return (_predicate == null) || _predicate.Invoke(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }

        /// <summary>Releases all references and subscriptions of the command.</summary>
        public void Dispose()
        {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Executes raising an event that the command should be requeried for its state.</summary>
        protected virtual void OnCanExecuteChanged()
        {
            var synchronizationContext = SynchronizationContext;

            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                synchronizationContext.Post(state => CanExecuteChanged?.Invoke(this, EventArgs.Empty), null);
            }
        }

        /// <summary>Releases all references and subscriptions of the command.</summary>
        /// <param name="disposing">Indicates whether the method was not invoked by finalyzer.</param>
        protected virtual void OnDispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            var trackingObject = _trackingObject;

            if (trackingObject != null)
            {
                trackingObject.PropertyChanged -= PropertyChangedEventHandler;
            }

            _trackingObject = null;
            _trackingProperties = null;
            _predicate = null;
            _action = null;

            SynchronizationContext = null;

            var subscribers = CanExecuteChanged?.GetInvocationList();

            if (subscribers != null)
            {
                for (var i = 0; i < subscribers.Length; i++)
                {
                    CanExecuteChanged -= (EventHandler)subscribers[i];
                }
            }
        }

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_trackingProperties?.ContainsKey(e.PropertyName) == true)
            {
                OnCanExecuteChanged();
            }
        }

        /// <summary>Gets the synchronization context to interact with UI through.</summary>
        public SynchronizationContext SynchronizationContext
        {
            get;
            set;
        }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged;
    }
}