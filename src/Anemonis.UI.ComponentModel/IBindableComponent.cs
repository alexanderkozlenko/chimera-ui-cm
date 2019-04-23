// © Alexander Kozlenko. Licensed under the MIT License.

using System.Threading;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable component.</summary>
    public interface IBindableComponent
    {
        /// <summary>Gets the synchronization context for interaction with UI.</summary>
        SynchronizationContext SynchronizationContext
        {
            get;
        }
    }
}
