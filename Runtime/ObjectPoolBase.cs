using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;


namespace Dythervin.ObjectPool
{
    [Serializable]
    public abstract class ObjectPoolBase<T> : IDisposable, IObjectPool<T>
        where T : class
    {
        public const int DefaultMaxSize = 1024;
        public const int DefaultCapacity = 16;
        public const bool DefaultCollectionCheck = true;

        public event Action<T> OnCreated;

        public event Action<T> OnDestroy;

        public event Action<T> OnGet;

        public event Action<T> OnRelease;

        [SerializeField] protected int maxSize = DefaultMaxSize;
        [SerializeField] protected bool collectionCheckDefault = DefaultCollectionCheck;
        private Stack<T> _stack;

        public int CountActive => CountAll - _stack.Count;

        public int CountAll { get; private set; }

        public int CountInactive => _stack.Count;

        public int MaxSize
        {
            get => maxSize;
            set
            {
                if (maxSize <= 0)
                    throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize));

                maxSize = value;
            }
        }

        protected Stack<T> Stack => _stack;

        protected ObjectPoolBase([DefaultValue(DefaultCollectionCheck)] bool collectionCheckDefault,
            Action<T> onGet = null, Action<T> onRelease = null, Action<T> actionOnDestroy = null,
            int defaultCapacity = DefaultCapacity, int maxSize = DefaultMaxSize)
        {
            SetStack(defaultCapacity);
            MaxSize = maxSize;
            OnGet = onGet;
            OnRelease = onRelease;
            OnDestroy = actionOnDestroy;
            this.collectionCheckDefault = collectionCheckDefault;
        }

        protected ObjectPoolBase() { }

        public ObjectPoolBase<T> EnsureObjCount(int count)
        {
            if (count <= 0 || count > maxSize)
                throw new ArgumentOutOfRangeException();

            while (_stack.Count < count)
            {
                T obj = GetNew();
                Release(ref obj);
            }

            return this;
        }

        protected void OnGot(T obj)
        {
            var onGet = OnGet;
            onGet?.Invoke(obj);
        }

        protected void SetStack(int capacity)
        {
            Assert.IsNull(_stack);
            _stack = new Stack<T>(capacity);
        }

        protected void OnDestroyInvoke(T element)
        {
            var actionOnDestroy = OnDestroy;
            actionOnDestroy?.Invoke(element);
        }

        protected void OnReleaseInvoke(T element)
        {
            var actionOnRelease = OnRelease;
            actionOnRelease?.Invoke(element);
        }

        protected T GetNew()
        {
            T obj = CreateNew();
            ++CountAll;
            var onCreated = OnCreated;
            onCreated?.Invoke(obj);
            return obj;
        }

        protected abstract T CreateNew();

        protected virtual void OnReleased(T element) { }

        public void Dispose()
        {
            Clear();
        }

        public virtual void Clear()
        {
            if (OnDestroy != null)
                foreach (T obj in _stack)
                    OnDestroy(obj);

            _stack.Clear();
            CountAll = 0;
        }

        object IObjectPool.Get()
        {
            return Get();
        }

        public PooledObjectHandler Get(out object obj)
        {
            obj = Get();
            return new PooledObjectHandler(this, obj);
        }

        public void Release(ref object element)
        {
            Release(ref element, collectionCheckDefault);
        }

        public void Release(ref object element, bool collectionCheck)
        {
            T tObj = (T)element;
            Release(ref tObj, collectionCheck);
            element = tObj;
        }

        public virtual T Get()
        {
            T obj = _stack.Count == 0 ? GetNew() : _stack.Pop();
            OnGot(obj);
            return obj;
        }

        // ReSharper disable once UnusedParameter.Global
        public void Release(ref T element, bool collectionCheck)
        {
            Debug.Assert(element != null);

            if (collectionCheck && _stack.Count > 0 && _stack.Contains(element))
                throw new InvalidOperationException(
                    "Trying to release an object that has already been released to the pool.");

            OnReleaseInvoke(element);

            if (_stack.Count < maxSize)
                _stack.Push(element);
            else
                OnDestroyInvoke(element);

            OnReleased(element);
            element = null;
        }

        public PooledObjectHandler<T> Get(out T obj)
        {
            obj = Get();
            return new PooledObjectHandler<T>(this, obj);
        }

        public void Release(ref T element)
        {
            Release(ref element, collectionCheckDefault);
        }
    }
}