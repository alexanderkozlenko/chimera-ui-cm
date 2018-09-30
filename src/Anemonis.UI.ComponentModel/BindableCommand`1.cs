// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Threading;
using System.Windows.Input;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a bindable command component.</summary>
    /// <typeparam name="T">The type of the parameter for action and predicate.</typeparam>
    public partial class BindableCommand<T> : IBindableCommand
    {
        private readonly Action<T> _actionMethod;
        private readonly Predicate<T> _predicateMethod;
        private readonly SynchronizationContext _synchronizationContext;

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

        bool ICommand.CanExecute(object parameter)
        {
            return (_predicateMethod == null) || (_predicateMethod.Invoke((T)parameter));
        }

        void ICommand.Execute(object parameter)
        {
            _actionMethod.Invoke((T)parameter);
        }

        private void UnsafeRaiseCanExecuteChanged(SynchronizationContext synchronizationContext)
        {
            if ((synchronizationContext == null) || (synchronizationContext == SynchronizationContext.Current))
            {
                UnsafeRaiseCanExecuteChanged();
            }
            else
            {
                var adapter = new SynchronizationContextAdapter(UnsafeRaiseCanExecuteChanged);

                synchronizationContext.Post(adapter.Post, null);
            }
        }

        private protected virtual void UnsafeRaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Raises an event that the command should be required for its state.</summary>
        public virtual void RaiseCanExecuteChanged()
        {
            UnsafeRaiseCanExecuteChanged(_synchronizationContext);
        }

        /// <summary>Releases all subscriptions to the command state changed event and to the property changed event of the currently observing objects.</summary>
        public virtual void Dispose()
        {
            CanExecuteChanged = null;
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