// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.ComponentModel;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable object component with advanced composition abilities.</summary>
    public interface IObservableObject : IBindableObject, IObservable<PropertyChangedEventArgs>
    {
    }
}
