namespace Dythervin.ObjectPool
{
    public sealed class SharedPool<T> : ObjectPoolAuto<T>
        where T : class, new()
    {
        public static readonly SharedPool<T> Instance = new SharedPool<T>();

        private SharedPool()
        {
        }
    }
}