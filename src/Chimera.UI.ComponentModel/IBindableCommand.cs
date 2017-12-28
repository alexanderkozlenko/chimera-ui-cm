using System;
using System.Threading;
using System.Windows.Input;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Defines a bindable command component.</summary>
    public interface IBindableCommand : ICommand, IDisposable
    {
        /// <summary>Raises an event that the command should be requeried for its state.</summary>
        void RaiseCanExecuteChanged();

        /// <summary>Starts tracking a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance of <see cref="IBindableCommand" />.</returns>
        IBindableCommand StartTrackingProperty(string propertyName);

        /// <summary>Stops tracking a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The current instance of <see cref="IBindableCommand" />.</returns>
        IBindableCommand StopTrackingProperty(string propertyName);

        /// <summary>Gets the synchronization context to interact with UI through.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}