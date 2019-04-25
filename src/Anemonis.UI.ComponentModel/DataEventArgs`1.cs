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

        internal DataEventArgs(string channelName, T value)
        {
            _channelName = channelName;
            _value = value;
        }

        /// <summary>Indicates whether the current <see cref="DataEventArgs{T}" /> is equal to the specified object.</summary>
        /// <param name="obj">The object to compare with the current <see cref="DataEventArgs{T}" />.</param>
        /// <returns><see langword="true" /> if the current <see cref="DataEventArgs{T}" /> is equal to the specified object; otherwise, <see langword="false" />.</returns>
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

        /// <summary>Returns the hash code for the current <see cref="DataEventArgs{T}" />.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)2166136261;

                hashCode ^= _channelName.GetHashCode();
                hashCode *= 16777619;
                hashCode ^= _value.GetHashCode();
                hashCode *= 16777619;

                return hashCode;
            }
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
