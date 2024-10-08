using Microsoft.Extensions.Caching.Memory;

namespace CachingProxy
{
    public static class Cache
    {
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());


        public static void AddToCache(string address, HttpResponseMessage response)
        {
            _cache.Set(address, response);
        }

        public static HttpResponseMessage GetFromCache(string address)
        {
            if (_cache.TryGetValue(address, out HttpResponseMessage? result))
                return result;

            return null;
        }
    }
}
