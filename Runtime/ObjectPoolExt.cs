namespace Dythervin.ObjectPool
{
    public static class ObjectPoolExt
    {
        public static void TryRelease<T>(this IObjectPool<T> pool, ref T obj) where T : class
        {
            if (obj != null)
                pool.Release(ref obj);
        }

        public static void TryRelease<T>(this IObjectPool<T> pool, ref T obj, bool collectionCheck) where T : class
        {
            if (obj != null)
                pool.Release(ref obj, collectionCheck);
        }
    }
}