﻿// © Alexander Kozlenko. Licensed under the MIT License.

using System.ComponentModel;

#pragma warning disable CA1030

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a bindable object component.</summary>
    public interface IBindableObject : IBindableComponent, INotifyPropertyChanged
    {
        /// <summary>Subscribes the current instance to the required notifications.</summary>
        void Subscribe();

        /// <summary>Unsubscribes the current instance from the required notifications.</summary>
        void Unsubscribe();
    }
}

#pragma warning restore CA1030
