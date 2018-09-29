// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a data events broker.</summary>
    public sealed class DataEventBroker : IDataEventBroker
    {
        private readonly object _syncRoot = new object();
        private readonly IDictionary<string, ISet<object>> _subscriptions = new Dictionary<string, ISet<object>>(StringComparer.Ordinal);

        /// <summary>Subscribes to channel events.</summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="channelName" /> or <paramref name="eventHandler" /> is <see langword="null" />.</exception>
        public void Subscribe<T>(string channelName, Action<DataEventArgs<T>> eventHandler)
        {
            if (channelName == null)
            {
                throw new ArgumentNullException(nameof(channelName));
            }
            if (eventHandler == null)
            {
                throw new ArgumentNullException(nameof(eventHandler));
            }

            lock (_syncRoot)
            {
                if (!_subscriptions.TryGetValue(channelName, out var channelSubscriptions))
                {
                    channelSubscriptions = new HashSet<object>(ReferenceEqualityComparer.Instance);
                    _subscriptions.Add(channelName, channelSubscriptions);
                }

                channelSubscriptions.Add(eventHandler);
            }
        }

        /// <summary>Unsubscribes from channel events.</summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="channelName" /> or <paramref name="eventHandler" /> is <see langword="null" />.</exception>
        public void Unsubscribe<T>(string channelName, Action<DataEventArgs<T>> eventHandler)
        {
            if (channelName == null)
            {
                throw new ArgumentNullException(nameof(channelName));
            }
            if (eventHandler == null)
            {
                throw new ArgumentNullException(nameof(eventHandler));
            }

            lock (_syncRoot)
            {
                if (!_subscriptions.TryGetValue(channelName, out var channelSubscriptions))
                {
                    return;
                }

                channelSubscriptions.Remove(eventHandler);

                if (channelSubscriptions.Count == 0)
                {
                    _subscriptions.Remove(channelName);
                }
            }
        }

        /// <summary>Publishes event data to all channel subscribers.</summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="value">The event data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="channelName" /> is <see langword="null" />.</exception>
        public void Publish<T>(string channelName, T value)
        {
            if (channelName == null)
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            var eventHandlerArray = default(object[]);

            lock (_syncRoot)
            {
                if (!_subscriptions.TryGetValue(channelName, out var eventHandlers))
                {
                    return;
                }

                eventHandlerArray = new object[eventHandlers.Count];
                eventHandlers.CopyTo(eventHandlerArray, 0);
            }

            for (var i = 0; i < eventHandlerArray.Length; i++)
            {
                if (eventHandlerArray[i] is Action<DataEventArgs<T>> eventHandler)
                {
                    eventHandler.Invoke(new DataEventArgs<T>(channelName, value));
                }
            }
        }

        /// <summary>Releases all event subscriptions.</summary>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                _subscriptions.Clear();
            }
        }
    }
}