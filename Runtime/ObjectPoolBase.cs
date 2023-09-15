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

        protected ObjectPoolBase() : this(DefaultCollectionCheck)
        {
        }

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

        protected void DestroyObj(T element)
        {
            var actionOnDestroy = OnDestroy;
            actionOnDestroy?.Invoke(element);
            CountAll--;
        }

        protected void OnObjectReleased(T element)
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

        public void Dispose()
        {
            Clear(1);
        }

        object IObjectPool.Get()
        {
            return Get();
        }

        void IObjectPool.Release(ref object element)
        {
            Release((T)element, collectionCheckDefault);
            element = null;
        }

        void IObjectPool.Release(ref object element, bool collectionCheck)
        {
            Release((T)element, collectionCheck);
            element = null;
        }

        void IObjectPool.Release(object element)
        {
            Release((T)element, collectionCheckDefault);
        }

        void IObjectPool.Release(object element, bool collectionCheck)
        {
            Release((T)element, collectionCheck);
        }

        public void Clear(float percent = 1)
        {
            if (percent > 1 || percent <= 0)
            {
                Debug.LogError($"{nameof(percent)} must be in range (0.0; 1.0]");
                return;
            }

            int count = (int)(_stack.Count * percent);
            for (int i = 0; i < count; i++)
            {
                T obj = _stack.Pop();
                DestroyObj(obj);
            }
        }

        public virtual T Get()
        {
            T obj = _stack.Count == 0 ? GetNew() : _stack.Pop();
            OnGot(obj);
            return obj;
        }

        public void Release(ref T element, bool collectionCheck)
        {
            Release(element, collectionCheck);

            element = null;
        }

        public void Release(T element)
        {
            Release(element, collectionCheckDefault);
        }

        public void Release(T element, bool collectionCheck)
        {
            Assert.IsNotNull(element);

            if (collectionCheck && _stack.Count > 0 && _stack.Contains(element))
                throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");

            OnObjectReleased(element);

            if (_stack.Count < maxSize)
                _stack.Push(element);
            else
                DestroyObj(element);
        }

        public void Release(ref T element)
        {
            Release(ref element, collectionCheckDefault);
        }
    }
}