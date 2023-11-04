
using Microsoft.Extensions.Logging;
using Schurko.Foundation.Caching.Memory;
using Schurko.Foundation.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Composition;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Caching
{
    [ExportMetadata("CacheType", "Memory")]
    public class MemoryCacheProvider : ICacheProvider
    {
        private static readonly ConcurrentDictionary<string, MemoryCacheProvider> _instances = new ConcurrentDictionary<string, MemoryCacheProvider>();
        private readonly ConcurrentDictionary<string, MemoryCacheItem> _dictionary = new ConcurrentDictionary<string, MemoryCacheItem>();
        private readonly Dictionary<string, IList<string>> _tagDictionary = new Dictionary<string, IList<string>>();
        private readonly TimeSpan _defaultAbsoluteExpiryInterval = new TimeSpan(24, 0, 0);
        private readonly TimeSpan _defaultSlidingExpiryInterval = TimeSpan.MaxValue;
        private readonly MemoryCacheStatistics _cacheStatistics = new MemoryCacheStatistics();

        public static MemoryCacheProvider GetInstance(string cacheName = null) => _instances.GetOrAdd(cacheName ?? string.Empty, s => new MemoryCacheProvider(cacheName));

        private MemoryCacheProvider(string cacheName = null) => CacheName = cacheName;

        public string CacheName { get; set; }

        public string CacheDescription { get; set; }

        public string CacheProviderDescription => "Provider which will cache objects in Memory.  This provider does not require the objects to be serializable";

        public ICacheStatistics Statistics => _cacheStatistics;

        public CacheProviderStatus ProviderStatus => CacheProviderStatus.Ready;

        public TObject Add<TObject>(
          string key,
          TObject item,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null)
        {
            MemoryCacheItemExpiry expiryInfo = CalculateExpirationDetails(expiration);
            if (!(tags is string[] strArray))
                strArray = tags == null ? (string[])null : tags.ToArray();
            string[] tagArray = strArray;
            ClearTags(key);
            ConcurrentDictionary<string, MemoryCacheItem> dictionary = _dictionary;
            string key1 = key;
            MemoryCacheItem addValue = new MemoryCacheItem();
            addValue.Item = item;
            addValue.Expiry = expiryInfo;
            addValue.Tags = tagArray;
            Func<string, MemoryCacheItem, MemoryCacheItem> updateValueFactory = (keyToFind, oldItem) => new MemoryCacheItem()
            {
                Item = item,
                Expiry = expiryInfo,
                Tags = tagArray
            };
            MemoryCacheItem memoryCacheItem = dictionary.AddOrUpdate(key1, addValue, updateValueFactory);
            UpdateTags(key, tagArray);
            _cacheStatistics.SetItemCount(_dictionary.Count);
            return (TObject)memoryCacheItem.Item;
        }

        public object Get(string key)
        {
            MemoryCacheItem memoryCacheItem;
            if (_dictionary.TryGetValue(key, out memoryCacheItem))
            {
                if (DateTime.Now > memoryCacheItem.Expiry.AbsoluteExpiryTime || DateTime.Now > memoryCacheItem.Expiry.SlidingExpiryTime)
                {
                    _dictionary.TryRemove(key, out memoryCacheItem);
                    _cacheStatistics.Miss();
                    return null;
                }
                _cacheStatistics.Hit();
                return memoryCacheItem.Item;
            }
            _cacheStatistics.Miss();
            return null;
        }

        public IEnumerable<KeyValuePair<string, object>> GetByTag(string tag) => 
            _tagDictionary.Where(kvp => tag == kvp.Key).SelectMany(item => item.Value, (item, key) => key)
            .Distinct().Select(key => new KeyValuePair<string, object>(key, Get(key)));

        public IEnumerable<KeyValuePair<string, object>> GetByAnyTag(IEnumerable<string> tags) => 
            tags == null ? Enumerable.Empty<KeyValuePair<string, object>>() : 
            _tagDictionary.Where(kvp => tags.Any(tag => tag == kvp.Key))
            .SelectMany(item => item.Value, (item, key) => key).Distinct()
            .Select(key => new KeyValuePair<string, object>(key, Get(key)));

        public IEnumerable<KeyValuePair<string, object>> GetByAllTags(IEnumerable<string> tags)
        {
            if (tags == null)
                return Enumerable.Empty<KeyValuePair<string, object>>();
            if (!(tags is string[] strArray))
                strArray = tags.ToArray();
            string[] tagArray = strArray;
            return _dictionary.Where(kvp => kvp.Value.Tags != null && kvp.Value.Tags.Count() == tagArray.Count() 
                && kvp.Value.Tags.All(t => tagArray.Any(tag => tag == t)))
                .Select(kvp => new KeyValuePair<string, object>(kvp.Key, Get(kvp.Key)));
        }

        public TObject GetOrAdd<TObject>(
          string key,
          Func<TObject> getItem,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null)
        {
            MemoryCacheItemExpiry expiryInfo = CalculateExpirationDetails(expiration);
            if (!(tags is string[] strArray))
                strArray = tags == null ? (string[])null : tags.ToArray();
            string[] tagArray = strArray;
            bool missed = false;
            MemoryCacheItem memoryCacheItem = _dictionary.GetOrAdd(key, keyToFind => new MemoryCacheItem()
            {
                Item = InjectMiss(getItem, out missed),
                Expiry = expiryInfo,
                Tags = tagArray
            });
            if (DateTime.Now > memoryCacheItem.Expiry.AbsoluteExpiryTime || DateTime.Now > memoryCacheItem.Expiry.SlidingExpiryTime)
            {
                ClearTags(key);
                ConcurrentDictionary<string, MemoryCacheItem> dictionary = _dictionary;
                string key1 = key;
                MemoryCacheItem addValue = new MemoryCacheItem();
                addValue.Item = InjectMiss(getItem, out missed);
                addValue.Expiry = expiryInfo;
                addValue.Tags = tagArray;
                Func<string, MemoryCacheItem, MemoryCacheItem> updateValueFactory = (keyToFind, oldItem) => new MemoryCacheItem()
                {
                    Item = InjectMiss(getItem, out missed),
                    Expiry = expiryInfo,
                    Tags = tagArray
                };
                memoryCacheItem = dictionary.AddOrUpdate(key1, addValue, updateValueFactory);
            }
            if (!missed)
                _cacheStatistics.Hit();
            UpdateTags(key, tagArray);
            _cacheStatistics.SetItemCount(_dictionary.Count);
            return (TObject)memoryCacheItem.Item;
        }

        public bool Remove(string key)
        {
            MemoryCacheItem memoryCacheItem;
            bool flag = _dictionary.TryRemove(key, out memoryCacheItem);
            if (memoryCacheItem != null && memoryCacheItem.Tags != null)
            {
                foreach (string tag in memoryCacheItem.Tags)
                {
                    if (_tagDictionary.ContainsKey(tag))
                        _tagDictionary[tag].Remove(key);
                    if (_tagDictionary[tag].Count == 0)
                        _tagDictionary.Remove(tag);
                }
            }
            _cacheStatistics.SetItemCount(_dictionary.Count);
            return flag;
        }

        public void Clear()
        {
            _cacheStatistics.Flush();
            _dictionary.Clear();
            _tagDictionary.Clear();
            _cacheStatistics.SetItemCount(_dictionary.Count);
        }

        private void ClearTags(string key)
        {
            if (!_dictionary.ContainsKey(key) || _dictionary[key].Tags == null)
                return;
            foreach (string tag in _dictionary[key].Tags)
            {
                if (_tagDictionary.ContainsKey(tag))
                    _tagDictionary[tag].Remove(key);
                if (_tagDictionary[tag].Count == 0)
                    _tagDictionary.Remove(tag);
            }
        }

        private void UpdateTags(string key, IEnumerable<string> tags = null)
        {
            if (tags == null)
                return;
            foreach (string tag in tags)
            {
                if (!_tagDictionary.ContainsKey(tag))
                    _tagDictionary.Add(tag, new List<string>());
                if (!_tagDictionary[tag].Contains(key))
                    _tagDictionary[tag].Add(key);
            }
        }

        private MemoryCacheItemExpiry CalculateExpirationDetails(CacheExpiration expiration)
        {
            expiration = expiration ?? Expiration.Default();
            TimeSpan timeSpan1 = expiration.UseDefaultExpiration ? _defaultAbsoluteExpiryInterval : expiration.ExpirationInterval;
            DateTime dateTime1 = timeSpan1 == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now.Add(timeSpan1);
            TimeSpan timeSpan2 = expiration.UseDefaultSlidingInterval ? _defaultSlidingExpiryInterval : expiration.SlidingInterval;
            DateTime dateTime2 = timeSpan2 == TimeSpan.MaxValue ? DateTime.MaxValue : DateTime.Now.Add(timeSpan2);
            return new MemoryCacheItemExpiry()
            {
                AbsoluteExpiryTime = dateTime1,
                SlidingExpiryTime = dateTime2,
                SlidingInterval = timeSpan2
            };
        }

        private T InjectMiss<T>(Func<T> getFunc, out bool missed)
        {
            missed = true;
            _cacheStatistics.Miss();
            try
            {
                return getFunc();
            }
            catch (Exception ex)
            {
                Log.Logger.LogError("Exception while creating item to cache", ex);
                throw;
            }
        }
    }
}
