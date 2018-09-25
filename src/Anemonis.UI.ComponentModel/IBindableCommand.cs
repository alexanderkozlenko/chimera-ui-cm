// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable command component.</summary>
    public interface IBindableCommand : ICommand, IDisposable
    {
        /// <summary>Raises an event that the command should be required for its state.</summary>
        void RaiseCanExecuteChanged();

        /// <summary>Adds an object to the collection of objects observed for the property change event, that triggers the event for command state re-querying.</summary>
        /// <param name="observable">The object to observe the property change event for.</param>
        void AddObservingObject(INotifyPropertyChanged observable);

        /// <summary>Adds an object to the collection of objects observed for the property change event, that triggers the event for command state re-querying.</summary>
        /// <param name="observable">The object to observe the property change event for.</param>
        /// <param name="propertyNames">The list of property names to observe.</param>
        void AddObservingObject(INotifyPropertyChanged observable, params string[] propertyNames);

        /// <summary>Removes an object from the collection of objects observed for the property change event, that triggers the event for command state re-querying.</summary>
        /// <param name="observable">The object to observe the property change event for.</param>
        void RemoveObservingObject(INotifyPropertyChanged observable);

        /// <summary>Gets the synchronization context for interaction with UI.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}