using System;

namespace Dythervin.ObjectPool
{
    public interface IObjectPool
    {
        int CountInactive { get; }

        void Clear();

        object Get();

        PooledObjectHandler Get(out object obj);


        void Release(ref object element);

        public void Release(ref object element, bool collectionCheck);
    }

    public interface IObjectPoolOut<out T> : IObjectPool where T : class
    {
        event Action<T> OnCreated;

        event Action<T> OnGet;

        event Action<T> OnRelease;

        event Action<T> OnDestroy;

        new T Get();
    }


    public interface IObjectPool<T> : IObjectPoolOut<T> where T : class
    {
        PooledObjectHandler<T> Get(out T obj);

        void Release(ref T obj);

        public void Release(ref T element, bool collectionCheck);
    }
}