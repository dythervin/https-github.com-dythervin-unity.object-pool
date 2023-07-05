using System;
using UnityEngine;

namespace Dythervin.ObjectPool
{
    public struct PooledObjectHandler : IDisposable
    {
        public bool IsValid { get; private set; }

        private readonly IObjectPool _pool;
        private object _obj;

        public readonly object Object
        {
            get
            {
                AssertNotDisposed();
                return _obj;
            }
        }

        internal PooledObjectHandler(IObjectPool pool, object obj)
        {
            _pool = pool;
            _obj = obj;
            IsValid = true;
        }

        public void Dispose()
        {
            AssertNotDisposed();
            _pool.Release(ref _obj);
            IsValid = false;
        }
        
        private readonly void AssertNotDisposed()
        {
            if (!IsValid)
                throw new ObjectDisposedException("Handler");
        }
    }


    public struct PooledObjectHandler<T> : IDisposable where T : class
    {
        public bool IsValid { get; private set; }

        private readonly IObjectPool<T> _pool;
        private T _obj;

        public readonly T Object
        {
            get
            {
                AssertNotDisposed();
                return _obj;
            }
        }

        internal PooledObjectHandler(IObjectPool<T> pool, T obj)
        {
            _pool = pool;
            _obj = obj;
            IsValid = true;
        }

        public void Dispose()
        {
            AssertNotDisposed();
            _pool.Release(ref _obj);
            IsValid = false;
        }
        
        private readonly void AssertNotDisposed()
        {
            if (!IsValid)
                throw new ObjectDisposedException("Handler");
        }
    }
}