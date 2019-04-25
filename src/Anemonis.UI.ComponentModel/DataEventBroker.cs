// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a data events broker.</summary>
    public class DataEventBroker : IDataEventBroker, IDisposable
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, HashSet<object>> _subscriptions = new Dictionary<string, HashSet<object>>(StringComparer.Ordinal);

        /// <summary>Initializes a new instance of the <see cref="DataEventBroker" /> class.</summary>
        public DataEventBroker()
        {
        }

        /// <summary />
        ~DataEventBroker()
        {
            Dispose(false);
        }

        /// <summary>Releases all event subscriptions.</summary>
        /// <param name="disposing">The value that indicates whether the method call comes from a <see cref="Dispose()"/> method (its value is <see langword="true" />) or from a finalizer (its value is <see langword="false" />).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_syncRoot)
                {
                    _subscriptions.Clear();
                }
            }
        }

        /// <summary>Releases all event subscriptions.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
                    channelSubscriptions = new HashSet<object>();
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
        public virtual void Publish<T>(string channelName, T value)
        {
            if (channelName == null)
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            lock (_syncRoot)
            {
                if (_subscriptions.TryGetValue(channelName, out var eventHandlers))
                {
                    var eventArgs = new DataEventArgs<T>(channelName, value);
                    var enumerator = eventHandlers.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current is Action<DataEventArgs<T>> eventHandler)
                        {
                            eventHandler.Invoke(eventArgs);
                        }
                    }
                }
            }
        }
    }
}
