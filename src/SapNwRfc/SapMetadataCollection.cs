using System;
using System.Collections;
using System.Collections.Generic;

namespace SapNwRfc
{
    /// <summary>
    /// Represents a collection of SAP metadata.
    /// </summary>
    /// <typeparam name="T">The metadata type.</typeparam>
    public sealed class SapMetadataCollection<T> : ISapMetadataCollection<T>
    {
        private readonly Func<int, T> _getByIndex;
        private readonly Func<string, T> _getByName;
        private readonly Func<int> _getCount;
        private int? _count;
        private T[] _cache;

        internal SapMetadataCollection(Func<int, T> getByIndex, Func<string, T> getByName, Func<int> getCount)
        {
            _getByIndex = getByIndex;
            _getByName = getByName;
            _getCount = getCount;
        }

        /// <summary>
        /// Gets metadata by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The metadata.</returns>
        public T this[int index]
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new T[Count];
                }

                T value = _cache[index];
                if (value == null)
                {
                    value = _getByIndex(index);
                    _cache[index] = value;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count => (_count ?? (_count = _getCount())).Value;

        /// <summary>
        /// Returns an enumerator that iterates through the metadata.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the metadata.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <inheritdoc cref="ISapMetadataCollection{T}"/>
        public bool TryGetValue(string name, out T value) => (value = _getByName(name)) != null;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
