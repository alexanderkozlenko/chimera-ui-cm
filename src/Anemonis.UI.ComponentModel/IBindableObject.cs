// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Threading;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable object component.</summary>
    public interface IBindableObject : INotifyPropertyChanged, IDisposable
    {
        /// <summary>Gets the synchronization context for interaction with UI.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}