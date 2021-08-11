using System;
using System.Collections;
using System.Collections.Generic;

namespace SapNwRfc.Internal
{
    internal sealed class MetadataList<T> : IReadOnlyList<T>
    {
        private readonly Func<int, T> _getByIndex;
        private readonly Func<int> _getCount;
        private int? _count;

        public MetadataList(Func<int, T> getByIndex, Func<int> getCount)
        {
            _getByIndex = getByIndex;
            _getCount = getCount;
        }

        public T this[int index] => _getByIndex(index);

        public int Count => (_count ?? (_count = _getCount())).Value;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
