// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Anemonis.UI.ComponentModel
{
    /// <summary>Represents data event arguments.</summary>
    /// <typeparam name="T">The type of event data.</typeparam>
    public readonly struct DataEventArgs<T> : IEquatable<DataEventArgs<T>>
    {
        private readonly string _channelName;
        private readonly T _value;

        /// <summary>Initializes a new instance of the <see cref="DataEventArgs{T}" /> structure.</summary>
        /// <param name="channelName">The name of the event channel.</param>
        /// <param name="value">The event data.</param>
        public DataEventArgs(string channelName, T value)
        {
            _channelName = channelName;
            _value = value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return (obj is DataEventArgs<T> other) && Equals(other);
        }

        /// <summary>Indicates whether the current <see cref="DataEventArgs{T}" /> is equal to another <see cref="DataEventArgs{T}" />.</summary>
        /// <param name="other">A <see cref="DataEventArgs{T}" /> to compare with the current <see cref="DataEventArgs{T}" />.</param>
        /// <returns><see langword="true" /> if the current <see cref="DataEventArgs{T}" /> is equal to the other <see cref="DataEventArgs{T}" />; otherwise, <see langword="false" />.</returns>
        public bool Equals(DataEventArgs<T> other)
        {
            return (_channelName == other._channelName) && EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(_channelName);
            hashCode.Add(_value);

            return hashCode.ToHashCode();
        }

        /// <summary>Indicates whether the left <see cref="DataEventArgs{T}" /> is equal to the right <see cref="DataEventArgs{T}" />.</summary>
        /// <param name="obj1">The left <see cref="DataEventArgs{T}" /> operand.</param>
        /// <param name="obj2">The right <see cref="DataEventArgs{T}" /> operand.</param>
        /// <returns><see langword="true" /> if the left <see cref="DataEventArgs{T}" /> is equal to the right <see cref="DataEventArgs{T}" />; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(DataEventArgs<T> obj1, DataEventArgs<T> obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>Indicates whether the left <see cref="DataEventArgs{T}" /> is not equal to the right <see cref="DataEventArgs{T}" />.</summary>
        /// <param name="obj1">The left <see cref="DataEventArgs{T}" /> operand.</param>
        /// <param name="obj2">The right <see cref="DataEventArgs{T}" /> operand.</param>
        /// <returns><see langword="true" /> if the left <see cref="DataEventArgs{T}" /> is not equal to the right <see cref="DataEventArgs{T}" />; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(DataEventArgs<T> obj1, DataEventArgs<T> obj2)
        {
            return !obj1.Equals(obj2);
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
