// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable object component.</summary>
    public interface IBindableObject : IBindableComponent, INotifyPropertyChanged, IDisposable
    {
        /// <summary>Subscribes the current instance to the required notifications.</summary>
        void Subscribe();

        /// <summary>Unsubscribes the current instance from the required notifications.</summary>
        void Unsubscribe();
    }
}