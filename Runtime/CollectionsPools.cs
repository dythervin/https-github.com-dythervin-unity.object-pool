using System.Collections.Generic;
using System.Text;

namespace Dythervin.ObjectPool
{
    public static class SharedPools
    {
        public static readonly SharedPool<StringBuilder> StringBuilder;

        public static IObjectPool<List<T>> GetListPool<T>() => CollectionsPools<List<T>, T>.Shared;

        public static IObjectPool<HashSet<T>> GetHashSetPool<T>() => CollectionsPools<HashSet<T>, T>.Shared;

        public static IObjectPool<Dictionary<TKey, TValue>> GetDictionaryPool<TKey, TValue>() =>
            CollectionsPools<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>.Shared;

        public static void GetListPool<T>(out IObjectPool<List<T>> pool) => pool = GetListPool<T>();

        public static void GetHashSetPool<T>(out IObjectPool<HashSet<T>> pool) => pool = GetHashSetPool<T>();

        public static void GetDictionaryPool<TKey, TValue>(out IObjectPool<Dictionary<TKey, TValue>> pool) =>
            pool = GetDictionaryPool<TKey, TValue>();

        static SharedPools()
        {
            StringBuilder = SharedPool<StringBuilder>.Instance;
            StringBuilder.OnRelease += builder => builder.Clear();
        }
    }

    public static class CollectionsPools<TCollection, T>
        where TCollection : class, ICollection<T>, new()
    {
        public static readonly SharedPool<TCollection> Shared;

        static CollectionsPools()
        {
            Shared = SharedPool<TCollection>.Instance;
            Shared.OnRelease += collection => collection.Clear();
        }
    }

    public static class ListPools<T>
    {
        public static readonly SharedPool<List<T>> Shared = CollectionsPools<List<T>, T>.Shared;
    }

    public static class HashSetPools<T>
    {
        public static readonly SharedPool<HashSet<T>> Shared = CollectionsPools<HashSet<T>, T>.Shared;
    }

    public static class DictionaryPools<TKey, TValue>
    {
        public static readonly SharedPool<Dictionary<TKey, TValue>> Shared =
            CollectionsPools<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>.Shared;
    }
}