namespace Dythervin.ObjectPool
{
    public static class ObjectPoolExt
    {
        public static PooledObjectHandler Get(this IObjectPool pool, out object obj)
        {
            obj = pool.Get();
            return new PooledObjectHandler(pool, obj);
        }

        public static PooledObjectHandler<T> Get<T>(this IObjectPool<T> pool, out T obj)
            where T : class
        {
            obj = pool.Get();
            return new PooledObjectHandler<T>(pool, obj);
        }

        public static void TryRelease<T>(this IObjectPool<T> pool, T obj)
            where T : class
        {
            if (obj != null)
                pool.Release(obj);
        }

        public static void TryRelease<T>(this IObjectPool<T> pool, T obj, bool collectionCheck)
            where T : class
        {
            if (obj != null)
                pool.Release(obj, collectionCheck);
        }

        public static void TryRelease<T>(this IObjectPool<T> pool, ref T obj)
            where T : class
        {
            if (obj != null)
                pool.Release(ref obj);
        }

        public static void TryRelease<T>(this IObjectPool<T> pool, ref T obj, bool collectionCheck)
            where T : class
        {
            if (obj != null)
                pool.Release(ref obj, collectionCheck);
        }
    }
}