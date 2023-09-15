using System;

namespace Dythervin.ObjectPool
{
    public interface IObjectPool
    {
        int CountInactive { get; }

        int CountActive { get; }

        object Get();

        void Release(ref object element);

        void Release(ref object element, bool collectionCheck);

        void Release(object element);

        void Release(object element, bool collectionCheck);

        void Clear(float percent = 1);
    }

    public interface IObjectPoolOut<out T> : IObjectPool
        where T : class
    {
        event Action<T> OnCreated;

        event Action<T> OnGet;

        event Action<T> OnRelease;

        event Action<T> OnDestroy;

        new T Get();
    }

    public interface IObjectPool<T> : IObjectPoolOut<T>
        where T : class
    {
        void Release(ref T obj);

        void Release(ref T element, bool collectionCheck);
        void Release(T element);

        void Release(T element, bool collectionCheck);
    }
}