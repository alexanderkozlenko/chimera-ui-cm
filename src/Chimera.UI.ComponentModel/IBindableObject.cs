// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Threading;

namespace Chimera.UI.ComponentModel
{
    /// <summary>Defines a bindable object component.</summary>
    public interface IBindableObject : INotifyPropertyChanged, IDisposable
    {
        /// <summary>Gets the synchronization context to interact with UI through.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}