// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Threading;
using System.Windows.Input;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Defines a bindable command component.</summary>
    public interface IBindableCommand : ICommand, IDisposable
    {
        /// <summary>Raises an event that the command should be required for its state.</summary>
        void RaiseCanExecuteChanged();

        /// <summary>Starts observing a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyNames">The names of the properties.</param>
        void StartObservingProperties(params string[] propertyNames);

        /// <summary>Stops observing a property for changing to raise an event about command's state.</summary>
        /// <param name="propertyNames">The names of the properties.</param>
        void StopObservingProperties(params string[] propertyNames);

        /// <summary>Gets the synchronization context to interact with UI through.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}