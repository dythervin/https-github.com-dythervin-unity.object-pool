using System.Collections.Generic;
using System.Text;


namespace Dythervin.ObjectPool
{
    public static class SharedPools
    {
        public static readonly IObjectPool<StringBuilder> StringBuilder =
            new ObjectPoolAuto<StringBuilder>(onRelease: builder => builder.Clear(), collectionCheckDefault: false);
    }


    public static class CollectionsPools<T>
    {
        public static readonly IObjectPool<List<T>> List =
            new ObjectPoolAuto<List<T>>(onRelease: list => list.Clear(), collectionCheckDefault: false);
    }
}