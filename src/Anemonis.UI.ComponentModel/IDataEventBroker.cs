// © Alexander Kozlenko. Licensed under the MIT License.

using System;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Defines a data events broker.</summary>
    public interface IDataEventBroker
    {
        /// <summary>Subscribes to channel events.</summary>
        /// <typeparam name="T">The type of event data.</typeparam>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="eventHandler">The event handler.</param>
        void Subscribe<T>(string channelName, Action<DataEventArgs<T>> eventHandler);

        /// <summary>Unsubscribes from channel events.</summary>
        /// <typeparam name="T">The type of event data.</typeparam>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="eventHandler">The event handler.</param>
        void Unsubscribe<T>(string channelName, Action<DataEventArgs<T>> eventHandler);

        /// <summary>Publishes event data to all channel subscribers.</summary>
        /// <typeparam name="T">The type of event data.</typeparam>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="value">The event data.</param>
        void Publish<T>(string channelName, T value);
    }
}
