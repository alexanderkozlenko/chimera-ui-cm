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

        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        void SubscribePropertyChanged(INotifyPropertyChanged observable);

        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <param name="propertyNames">The list of property names to handle.</param>
        void SubscribePropertyChanged(INotifyPropertyChanged observable, params string[] propertyNames);

        /// <summary>Unsubscribes from property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        void UnsubscribePropertyChanged(INotifyPropertyChanged observable);

        /// <summary>Gets the synchronization context for interaction with UI.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}