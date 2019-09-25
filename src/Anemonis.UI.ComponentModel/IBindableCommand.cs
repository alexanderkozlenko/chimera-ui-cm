// © Alexander Kozlenko. Licensed under the MIT License.

using System.Windows.Input;

#pragma warning disable CA1030

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable command component.</summary>
    public interface IBindableCommand : IBindableComponent, ICommand
    {
        /// <summary>Raises an event that the command should be required for its state.</summary>
        void RaiseCanExecuteChanged();
    }
}
