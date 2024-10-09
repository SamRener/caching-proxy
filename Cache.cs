using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace CachingProxy
{
    public static class Cache
    {
        private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private static bool _useMemoryCache = false;
        private const string CACHE_PATH = ".cache";
        public static void AddToCache(string address, Response response)
        {
            if (_useMemoryCache) {
                _cache.Set(address, response);
                return;
            }

            var cachePath = Path.Combine(CACHE_PATH, address.Replace("\\", "").Replace("/", ""));
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            File.WriteAllText(Path.Combine(cachePath, "headers.json"), JsonSerializer.Serialize(response.Headers));
            File.WriteAllBytes(Path.Combine(cachePath, "data.dat"), response.Data);
        }

        public static Response GetFromCache(string address)
        {
            if (_useMemoryCache && _cache.TryGetValue(address, out Response? result))
                return result;

            var cachePath = Path.Combine(CACHE_PATH, address.Replace("\\", "").Replace("/", ""));
            if (!Directory.Exists(cachePath))
                return null;

            var headers = JsonSerializer.Deserialize<Headers>(File.ReadAllText(Path.Combine(cachePath, "headers.json")));
            var data = File.ReadAllBytes(Path.Combine(cachePath, "data.dat"));
            return new Response(headers, data);
        }

        internal static void ClearCache()
        {
            Console.WriteLine("Clearing the cache");
            if (_useMemoryCache)
                _cache.Clear();

            if (Directory.Exists(CACHE_PATH))
                Directory.Delete(CACHE_PATH, true);
        }

        internal static void ShouldUseMemoryCache(bool should)
        {
            _useMemoryCache = should;
        }
    }
}
