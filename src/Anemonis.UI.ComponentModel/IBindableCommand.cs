// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Threading;
using System.Windows.Input;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable command component.</summary>
    public interface IBindableCommand : ICommand, IDisposable
    {
        /// <summary>Raises an event that the command should be required for its state.</summary>
        void RaiseCanExecuteChanged();

        /// <summary>Gets the synchronization context for interaction with UI.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}