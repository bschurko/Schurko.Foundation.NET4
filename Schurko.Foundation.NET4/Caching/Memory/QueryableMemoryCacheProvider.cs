
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Composition;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Caching.Memory
{
    [ExportMetadata("CacheType", "Memory")]
    public class QueryableMemoryCacheProvider : IQueryableCacheProvider
    {
        private static readonly ConcurrentDictionary<string, MemoryCacheItem> _dictionary = new ConcurrentDictionary<string, MemoryCacheItem>();
        private static readonly QueryableMemoryCacheProvider _instance = new QueryableMemoryCacheProvider();
        private readonly TimeSpan _defaultExpiryTime = new TimeSpan(24, 0, 0);

        private QueryableMemoryCacheProvider()
        {
        }

        public static QueryableMemoryCacheProvider Instance => _instance;

        public event CacheChangeHandler OnCacheItemExpired;

        public IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query, TimeSpan cacheDuration)
        {
            string key1 = GetKey(query);
            MemoryCacheItemExpiry expiry = new MemoryCacheItemExpiry()
            {
                AbsoluteExpiryTime = DateTime.Now.Add(cacheDuration)
            };
            MemoryCacheItem memoryCacheItem = _dictionary.GetOrAdd(key1, keyToFind => new MemoryCacheItem()
            {
                Item = query.ToList(),
                Expiry = expiry
            });
            if (DateTime.Now > memoryCacheItem.Expiry.AbsoluteExpiryTime)
            {
                if (OnCacheItemExpired != null)
                    OnCacheItemExpired(this, memoryCacheItem);
                ConcurrentDictionary<string, MemoryCacheItem> dictionary = _dictionary;
                string key2 = key1;
                MemoryCacheItem addValue = new MemoryCacheItem();
                addValue.Item = memoryCacheItem.Item;
                addValue.Expiry = expiry;
                Func<string, MemoryCacheItem, MemoryCacheItem> updateValueFactory = (keyToFind, oldItem) => new MemoryCacheItem()
                {
                    Item = query.ToList(),
                    Expiry = expiry
                };
                memoryCacheItem = dictionary.AddOrUpdate(key2, addValue, updateValueFactory);
            }
            return (IEnumerable<T>)memoryCacheItem.Item;
        }

        public IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query) => GetOrCreateCache(query, _defaultExpiryTime);

        public bool RemoveFromCache<T>(IQueryable<T> query)
        {
            string key = GetKey(query);
            return _dictionary.TryRemove(key, out MemoryCacheItem _);
        }

        private static string GetKey<T>(IQueryable<T> query) => query.ToString() + "\n\r" + typeof(T).AssemblyQualifiedName;

        public delegate void CacheChangeHandler(object sender, MemoryCacheItem item);
    }
}
