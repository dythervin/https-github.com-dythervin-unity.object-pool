using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Dythervin.Core.Extensions;

namespace Dythervin.ObjectPool
{
    public static class ArrayPools
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IObjectPool<T[]> GetPool<T>(int lenght)
        {
            return ArrayPool<T>.GetPool(lenght);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Get<T>(int lenght)
        {
            return ArrayPool<T>.Get(lenght);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PooledObjectHandler<T[]> Get<T>(int lenght, out T[] array)
        {
            return GetPool<T>(lenght).Get(out array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release<T>(ref T[] array)
        {
            ArrayPool<T>.Release(ref array);
        }
    }

    internal class ArrayPool<T> : ObjectPoolBase<T[]>
    {
        private static readonly Dictionary<int, IObjectPool<T[]>> Pools = new Dictionary<int, IObjectPool<T[]>>()
        {
            { 0, new ObjectPool<T[]>(Array.Empty<T>, collectionCheckDefault: false) }
        };
        
        private readonly int _lenght;

        private readonly Action<T[]> _onRelease = typeof(T).IsClass ? obj => obj.Reset() : null;

        public ArrayPool(int lenght, bool collectionCheckDefault = DefaultCollectionCheck, Action<T[]> onGet = null,
            Action<T[]> onRelease = null, Action<T[]> actionOnDestroy = null, int defaultCapacity = DefaultCapacity,
            int maxSize = DefaultMaxSize) : base(collectionCheckDefault,
            onGet,
            onRelease,
            actionOnDestroy,
            defaultCapacity,
            maxSize)
        {
            _lenght = lenght;
        }

        public ArrayPool(int lenght)
        {
            _lenght = lenght;
        }

        public static IObjectPool<T[]> GetPool(int lenght)
        {
            if (lenght < 0)
                throw new ArgumentOutOfRangeException();

            if (!Pools.TryGetValue(lenght, out var pool))
            {
                Pools.Add(lenght, pool = new ArrayPool<T>(lenght));
            }

            return pool;
        }

        public static T[] Get(int lenght)
        {
            return GetPool(lenght).Get();
        }

        public new static void Release(ref T[] array)
        {
            GetPool(array.Length).Release(ref array);
        }

        protected override T[] CreateNew()
        {
            return _lenght == 0 ? Array.Empty<T>() : new T[_lenght];
        }
    }
}