using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace Dythervin.ObjectPool
{
    public static class ArrayPoolHandlerExt
    {
        public static PooledArrayHandler<T> ToPooledArray<T>(this IReadOnlyList<T> list)
        {
            return new PooledArrayHandler<T>(list);
        }

        public static PooledArrayHandler<T> ToPooledArray<T>(this HashSet<T> list)
        {
            return new PooledArrayHandler<T>(list);
        }
    }

    public struct PooledArrayHandler<T> : IDisposable, IReadOnlyList<T>
    {
        private static readonly bool ClearOnDispose = typeof(T).IsClass;
        private T[] _tempArray;

        public int Count { get; }

        public readonly T this[int index]
        {
            get
            {
                AssertIndex(index);
                return _tempArray[index];
            }
            set
            {
                AssertIndex(index);
                _tempArray[index] = value;
            }
        }

        internal PooledArrayHandler(IReadOnlyList<T> list) : this(list.Count)
        {
            for (int i = 0; i < Count; i++)
            {
                _tempArray[i] = list[i];
            }
        }

        public PooledArrayHandler(int count)
        {
            Count = count;
            _tempArray = ArrayPool<T>.Shared.Rent(Count);
        }

        public PooledArrayHandler(HashSet<T> hashSet) : this(hashSet.Count)
        {
            int i = 0;
            foreach (T value in hashSet)
            {
                _tempArray[i++] = value;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        private readonly void AssertIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(_tempArray, ClearOnDispose);
            _tempArray = null;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>
        {
            private PooledArrayHandler<T> _self;
            private int _index;

            public T Current
            {
                get
                {
                    if (_index < 0 || _index > _self.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(_index));
                    }

                    return _self._tempArray[_index];
                }
            }

            object IEnumerator.Current => Current;

            public Enumerator(PooledArrayHandler<T> self)
            {
                _self = self;
                _index = -1;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _self.Count;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {
                _self = default;
            }
        }
    }
}