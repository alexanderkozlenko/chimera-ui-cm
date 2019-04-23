// © Alexander Kozlenko. Licensed under the MIT License.

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents data event arguments.</summary>
    /// <typeparam name="T">The type of event data.</typeparam>
    public readonly struct DataEventArgs<T>
    {
        private readonly string _channelName;
        private readonly T _value;

        internal DataEventArgs(string channelName, T value)
        {
            _channelName = channelName;
            _value = value;
        }

        /// <summary>Gets the name of the event channel.</summary>
        public string ChannelName
        {
            get => _channelName;
        }

        /// <summary>Gets the event data.</summary>
        public T Value
        {
            get => _value;
        }
    }
}
