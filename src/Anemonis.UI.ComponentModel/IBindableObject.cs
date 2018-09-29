// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Threading;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable object component.</summary>
    public interface IBindableObject : INotifyPropertyChanged, IDisposable
    {
        /// <summary>Subscribes the current instance to the required notifications.</summary>
        void Subscribe();

        /// <summary>Unsubscribes the current instance from the required notifications.</summary>
        void Unsubscribe();

        /// <summary>Gets the synchronization context for interaction with UI.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}