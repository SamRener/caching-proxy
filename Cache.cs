using Microsoft.Extensions.Caching.Memory;

namespace CachingProxy
{
    public static class Cache
    {
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());


        public static void AddToCache(string address, Response response)
        {
            _cache.Set(address, response);
        }

        public static Response GetFromCache(string address)
        {
            if (_cache.TryGetValue(address, out Response? result))
                return result;

            return null;
        }
    }
}
