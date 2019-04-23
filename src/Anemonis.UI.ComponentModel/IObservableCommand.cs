// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable command component with advanced composition abilities.</summary>
    public interface IObservableCommand : IBindableCommand, IObservable<EventArgs>
    {
        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        void Subscribe(INotifyPropertyChanged observable);

        /// <summary>Subscribes for property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        /// <param name="propertyNames">The list of property names to handle.</param>
        void Subscribe(INotifyPropertyChanged observable, params string[] propertyNames);

        /// <summary>Unsubscribes from property changed events of an object to trigger the event for command state re-querying.</summary>
        /// <param name="observable">The object to handle property changed events of.</param>
        void Unsubscribe(INotifyPropertyChanged observable);
    }
}
