using Schurko.Foundation.Caching;
using Schurko.Foundation.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class QueryableCacheExtensions
    {
        private static IQueryableCacheProvider _cacheProvider;

        public static IQueryableCacheProvider CurrentProvider => _cacheProvider;

        static QueryableCacheExtensions() => SetCacheProvider(QueryableMemoryCacheProvider.Instance);

        public static void SetCacheProvider(IQueryableCacheProvider provider) => _cacheProvider = provider != null ? provider : throw new ArgumentNullException(nameof(provider), "Provider must not be null.");

        public static IEnumerable<T> AsCacheable<T>(this IQueryable<T> query)
        {
            ValidateCacheProvider();
            return _cacheProvider.GetOrCreateCache(query);
        }

        public static IEnumerable<T> AsCacheable<T>(this IQueryable<T> query, TimeSpan cacheDuration)
        {
            ValidateCacheProvider();
            return _cacheProvider.GetOrCreateCache(query, cacheDuration);
        }

        public static bool RemoveFromCache<T>(IQueryable<T> query)
        {
            ValidateCacheProvider();
            return _cacheProvider.RemoveFromCache(query);
        }

        private static void ValidateCacheProvider()
        {
            if (_cacheProvider == null)
                throw new InvalidOperationException("Please set cache provider (call SetCacheProvider) before using caching");
        }
    }
}
