// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents a UI data events broker.</summary>
    public sealed class DataEventBroker : IDataEventBroker
    {
        private readonly IDictionary<string, ISet<object>> _subscriptions = new Dictionary<string, ISet<object>>(StringComparer.Ordinal);
        private readonly object _subscriptionsLockRoot = new object();

        /// <summary>Subscribes to channel events.</summary>
        /// <typeparam name="T">The type of the event data.</typeparam>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="channelName" /> or <paramref name="eventHandler" /> is <see langword="null" />.</exception>
        public void Subscribe<T>(string channelName, Action<T> eventHandler)
        {
            if (channelName == null)
            {
                throw new ArgumentNullException(nameof(channelName));
            }
            if (eventHandler == null)
            {
                throw new ArgumentNullException(nameof(eventHandler));
            }

            lock (_subscriptionsLockRoot)
            {
                if (!_subscriptions.TryGetValue(channelName, out var channelSubscriptions))
                {
                    channelSubscriptions = new HashSet<object>(ReferenceEqualityComparer<object>.Instance);

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
        public void Unsubscribe<T>(string channelName, Action<T> eventHandler)
        {
            if (channelName == null)
            {
                throw new ArgumentNullException(nameof(channelName));
            }
            if (eventHandler == null)
            {
                throw new ArgumentNullException(nameof(eventHandler));
            }

            lock (_subscriptionsLockRoot)
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
        /// <exception cref="AggregateException">One or more subscribers throw an exception during event arguments handling.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="channelName" /> is <see langword="null" />.</exception>
        public void Publish<T>(string channelName, T value)
        {
            if (channelName == null)
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            var channelSubscriptionsArray = default(object[]);

            lock (_subscriptionsLockRoot)
            {
                if (!_subscriptions.TryGetValue(channelName, out var channelSubscriptions))
                {
                    return;
                }

                channelSubscriptionsArray = new object[channelSubscriptions.Count];
                channelSubscriptions.CopyTo(channelSubscriptionsArray, 0);
            }

            var exceptions = default(List<Exception>);

            for (var i = 0; i < channelSubscriptionsArray.Length; i++)
            {
                if (channelSubscriptionsArray[i] is Action<T> eventHandler)
                {
                    try
                    {
                        eventHandler.Invoke(value);
                    }
                    catch (Exception e)
                    {
                        if (exceptions == null)
                        {
                            exceptions = new List<Exception>();
                        }

                        exceptions.Add(e);
                    }
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>Releases all event subscriptions.</summary>
        public void Dispose()
        {
            lock (_subscriptionsLockRoot)
            {
                _subscriptions.Clear();
            }
        }
    }
}