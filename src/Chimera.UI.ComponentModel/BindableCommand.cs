using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Represents a bindable command component.</summary>
    public abstract class BindableCommand : IBindableCommand
    {
        private static Func<BindableCommand> _factory;

        private Action<object> _action;
        private Predicate<object> _predicate;
        private IBindableObject _trackingObject;
        private ConcurrentDictionary<string, byte> _trackingProperties;

        /// <summary>Initializes a new instance of the <see cref="BindableCommand" /> class.</summary>
        protected BindableCommand()
        {
        }

        /// <summary>Registers a factory for creating a platform-specific command instance.</summary>
        /// <param name="factory">The factory for command instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="factory" /> is <see langword="null" />.</exception>
        public static void RegisterFactory(Func<BindableCommand> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <summary>Creates a bindable command.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <returns>An <see cref="IBindableCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The command factory is not defined.</exception>
        public static IBindableCommand Create(Action<object> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateInternal(default(IBindableObject), action, null, null);
        }

        /// <summary>Creates a bindable command.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="synchronizationContext">The synchronization context for executing UI-related actions.</param>
        /// <returns>An <see cref="IBindableCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The command factory is not defined.</exception>
        public static IBindableCommand Create(Action<object> action, SynchronizationContext synchronizationContext)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateInternal(default(IBindableObject), action, null, synchronizationContext);
        }

        /// <summary>Creates a bindable command.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="predicate">The predicate to check if the command can be executed.</param>
        /// <returns>An <see cref="IBindableCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The command factory is not defined.</exception>
        public static IBindableCommand Create(Action<object> action, Predicate<object> predicate)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateInternal(default(IBindableObject), action, predicate, null);
        }

        /// <summary>Creates a bindable command.</summary>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="predicate">The predicate to check if the command can be executed.</param>
        /// <param name="synchronizationContext">The synchronization context for executing UI-related actions.</param>
        /// <returns>An <see cref="IBindableCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The command factory is not defined.</exception>
        public static IBindableCommand Create(Action<object> action, Predicate<object> predicate, SynchronizationContext synchronizationContext)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateInternal(default(IBindableObject), action, predicate, synchronizationContext);
        }

        /// <summary>Creates a bindable command.</summary>
        /// <typeparam name="TObject">The type of tracking object.</typeparam>
        /// <param name="trackingObject">The <see cref="IBindableObject" /> as a source of tracking properties and a synchronization context.</param>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <returns>An <see cref="IBindableCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> or <paramref name="trackingObject" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The command factory is not defined.</exception>
        public static IBindableCommand Create<TObject>(TObject trackingObject, Action<object> action)
            where TObject : IBindableObject
        {
            if (trackingObject == null)
            {
                throw new ArgumentNullException(nameof(trackingObject));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateInternal(trackingObject, action, null, null);
        }

        /// <summary>Creates a bindable command.</summary>
        /// <typeparam name="TObject">The type of tracking object.</typeparam>
        /// <param name="trackingObject">The <see cref="IBindableObject" /> as a source of tracking properties and a synchronization context.</param>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="predicate">The predicate to check if the command can be executed.</param>
        /// <returns>An <see cref="IBindableCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> or <paramref name="trackingObject" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The command factory is not defined.</exception>
        public static IBindableCommand Create<TObject>(TObject trackingObject, Action<object> action, Predicate<object> predicate)
            where TObject : IBindableObject
        {
            if (trackingObject == null)
            {
                throw new ArgumentNullException(nameof(trackingObject));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateInternal(trackingObject, action, predicate, null);
        }

        /// <summary>Creates a bindable command.</summary>
        /// <typeparam name="TObject">The type of tracking object.</typeparam>
        /// <param name="trackingObject">The <see cref="IBindableObject" /> as a source of tracking properties and a synchronization context.</param>
        /// <param name="action">The action to execute when the command is executed.</param>
        /// <param name="predicate">The predicate to check if the command can be executed.</param>
        /// <param name="synchronizationContext">The synchronization context for executing UI-related actions.</param>
        /// <returns>An <see cref="IBindableCommand" /> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> or <paramref name="trackingObject" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The command factory is not defined.</exception>
        public static IBindableCommand Create<TObject>(TObject trackingObject, Action<object> action, Predicate<object> predicate, SynchronizationContext synchronizationContext)
            where TObject : IBindableObject
        {
            if (trackingObject == null)
            {
                throw new ArgumentNullException(nameof(trackingObject));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return CreateInternal(trackingObject, action, predicate, synchronizationContext);
        }

        private static BindableCommand CreateInternal<TObject>(TObject trackingObject, Action<object> action, Predicate<object> predicate, SynchronizationContext synchronizationContext)
            where TObject : IBindableObject
        {
            if (_factory == null)
            {
                throw new InvalidOperationException("The command factory is not defined");
            }

            var command = _factory.Invoke();

            command._trackingObject = trackingObject;
            command._action = action;
            command._predicate = predicate;

            command.SynchronizationContext = synchronizationContext ?? trackingObject?.SynchronizationContext;

            if (trackingObject != null)
            {
                command._trackingProperties = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
                trackingObject.PropertyChanged += command.PropertyChangedEventHandler;
            }

            return command;
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
                throw new InvalidOperationException("The tracking object is not defined");
            }

            _trackingProperties?.TryAdd(propertyName, default(byte));

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
                throw new InvalidOperationException("The tracking object is not defined");
            }

            _trackingProperties?.TryRemove(propertyName, out var _);

            return this;
        }

        /// <summary>Raises an event that the command should be requeried for its state.</summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>Determines whether the command can execute in its current state.</summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <returns><see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.</returns>
        public bool CanExecute(object parameter)
        {
            return (_predicate == null) || _predicate.Invoke(parameter);
        }

        /// <summary>Executes the command.</summary>
        /// <param name="parameter">The data used by the command.</param>
        /// <exception cref="ObjectDisposedException">The instance is disposed.</exception>
        public void Execute(object parameter)
        {
            if (_action == null)
            {
                throw new ObjectDisposedException(nameof(BindableCommand));
            }

            _action.Invoke(parameter);
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
            if ((SynchronizationContext == null) || (SynchronizationContext == SynchronizationContext.Current))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                SynchronizationContext.Post(state => CanExecuteChanged?.Invoke(this, EventArgs.Empty), null);
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
        public SynchronizationContext SynchronizationContext { get; private set; }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged;
    }
}