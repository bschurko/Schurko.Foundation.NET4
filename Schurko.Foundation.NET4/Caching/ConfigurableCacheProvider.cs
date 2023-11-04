using Schurko.Foundation.Caching.Memory;
using Schurko.Foundation.IoC.MEF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Composition;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Caching
{
    [ExportMetadata("CacheType", "Configurable")]
    public class ConfigurableCacheProvider : ICacheProvider
    {
        private static readonly ConcurrentDictionary<string, ConfigurableCacheProvider> Instances = new ConcurrentDictionary<string, ConfigurableCacheProvider>();
        private static readonly MemoryCacheStatistics CacheStatistics = new MemoryCacheStatistics();
        private ICacheProvider _handler;
        private ConfigurableCacheProvider _nextProvider;
        private ConfigurableCacheSettings _settings;
        private bool _configured;

        public static ConfigurableCacheProvider GetInstance(string cacheName = null) => Instances.GetOrAdd(cacheName ?? string.Empty, s => new ConfigurableCacheProvider(cacheName));

        public ConfigurableCacheProvider() => _handler = GetInstance();

        private ConfigurableCacheProvider(string cacheName) => CacheName = cacheName;

        private ConfigurableCacheProvider(string cacheName, Queue<string> providerChain)
        {
            CacheName = cacheName;
            BuildChain(providerChain);
        }

        public ConfigurableCacheSettings Settings
        {
            get
            {
                ConfigurableCacheSettings settings = _settings ?? (_settings = new ConfigurableCacheSettings(CacheName));
                if (settings.CacheName != CacheName)
                    settings = _settings = new ConfigurableCacheSettings(CacheName);
                return settings;
            }
        }

        public string CacheName { get; set; }

        public string CacheDescription { get; set; }

        public string CacheProviderDescription => "Provider which will resolve configured Cache Providers to provider either a Fallback cache provider model which will the first provider in the chain that isn't in a faulted state, or a Cascading cache provider modelwhich will use multiple cache providers to try to reduce the need for regenerating the item to add.";

        public ICacheStatistics Statistics => CacheStatistics;

        public CacheProviderStatus ProviderStatus => CacheProviderStatus.Ready;

        public TObject Add<TObject>(
          string key,
          TObject item,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null)
        {
            EnsureCacheProviderIsConfigured();
            if (Settings.Mode == ConfigurableCacheProviderMode.Fallback)
            {
                TObject @object = default;
                if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                    @object = _handler.Add(key, item, expiration, tags);
                else if (!EndOfChain)
                    @object = _nextProvider.Add(key, item, expiration, tags);
                return @object;
            }
            if (Settings.Mode != ConfigurableCacheProviderMode.Cascading)
                throw new NotImplementedException(string.Format("Mode {0} has not implemented Add", Settings.Mode));
            string[] array = tags == null ? (string[])null : tags.ToArray();
            TObject object1 = _handler.Add(key, item, expiration, array);
            if (!EndOfChain)
                object1 = _nextProvider.Add(key, object1, expiration, array);
            return object1;
        }

        public object Get(string key)
        {
            EnsureCacheProviderIsConfigured();
            if (Settings.Mode == ConfigurableCacheProviderMode.Fallback)
            {
                object obj = null;
                if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                    obj = _handler.Get(key);
                else if (!EndOfChain)
                    obj = _nextProvider.Get(key);
                if (obj == null)
                    CacheStatistics.Miss();
                else
                    CacheStatistics.Hit();
                return obj;
            }
            if (Settings.Mode != ConfigurableCacheProviderMode.Cascading)
                throw new NotImplementedException(string.Format("Mode {0} has not implemented Get", Settings.Mode));
            object obj1 = _handler.Get(key);
            if (obj1 == null && !EndOfChain)
            {
                CacheStatistics.Miss();
                obj1 = _nextProvider.Get(key);
                _handler.Add(key, obj1);
            }
            else
                CacheStatistics.Miss();
            if (obj1 != null)
                CacheStatistics.Hit();
            return obj1;
        }

        public IEnumerable<KeyValuePair<string, object>> GetByTag(string tag)
        {
            EnsureCacheProviderIsConfigured();
            if (Settings.Mode != ConfigurableCacheProviderMode.Fallback)
                throw new NotImplementedException("GetByTag not implemented for Cascading Mode");
            IEnumerable<KeyValuePair<string, object>> byTag = null;
            if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                byTag = _handler.GetByTag(tag);
            else if (!EndOfChain)
                byTag = _nextProvider.GetByTag(tag);
            if (byTag == null)
                CacheStatistics.Miss();
            else
                CacheStatistics.Hit();
            return byTag;
        }

        public IEnumerable<KeyValuePair<string, object>> GetByAnyTag(IEnumerable<string> tags)
        {
            EnsureCacheProviderIsConfigured();
            if (Settings.Mode != ConfigurableCacheProviderMode.Fallback)
                throw new NotImplementedException("GetByAnyTag not implemented for Cascading Mode");
            IEnumerable<KeyValuePair<string, object>> byAnyTag = null;
            if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                byAnyTag = _handler.GetByAnyTag(tags);
            else if (!EndOfChain)
                byAnyTag = _nextProvider.GetByAnyTag(tags);
            if (byAnyTag == null)
                CacheStatistics.Miss();
            else
                CacheStatistics.Hit();
            return byAnyTag;
        }

        public IEnumerable<KeyValuePair<string, object>> GetByAllTags(IEnumerable<string> tags)
        {
            EnsureCacheProviderIsConfigured();
            if (Settings.Mode != ConfigurableCacheProviderMode.Fallback)
                throw new NotImplementedException("GetByAnyTag not implemented for Cascading Mode");
            IEnumerable<KeyValuePair<string, object>> byAllTags = null;
            if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                byAllTags = _handler.GetByAllTags(tags);
            else if (!EndOfChain)
                byAllTags = _nextProvider.GetByAllTags(tags);
            if (byAllTags == null)
                CacheStatistics.Miss();
            else
                CacheStatistics.Hit();
            return byAllTags;
        }

        public TObject GetOrAdd<TObject>(
          string key,
          Func<TObject> getItem,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null)
        {
            EnsureCacheProviderIsConfigured();
            bool missed = false;
            if (Settings.Mode == ConfigurableCacheProviderMode.Fallback)
            {
                TObject orAdd = default;
                if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                {
                    orAdd = _handler.GetOrAdd(key, () => InjectMiss(getItem, out missed), expiration, tags);
                    if (!missed)
                        CacheStatistics.Hit();
                }
                else if (!EndOfChain)
                    orAdd = _nextProvider.GetOrAdd(key, getItem, expiration, tags);
                return orAdd;
            }
            if (Settings.Mode != ConfigurableCacheProviderMode.Cascading)
                throw new NotImplementedException(string.Format("Mode {0} has not implemented GetOrAdd", Settings.Mode));
            TObject orAdd1 = _handler.GetOrAdd(key, () => EndOfChain ? InjectMiss(getItem, out missed) : _nextProvider.GetOrAdd(key, getItem, expiration, tags), expiration, tags);
            if (missed)
                return orAdd1;
            CacheStatistics.Hit();
            return orAdd1;
        }

        public bool Remove(string key)
        {
            EnsureCacheProviderIsConfigured();
            if (Settings.Mode == ConfigurableCacheProviderMode.Fallback)
            {
                if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                    return _handler.Remove(key);
                if (!EndOfChain)
                    return _nextProvider.Remove(key);
            }
            if (Settings.Mode != ConfigurableCacheProviderMode.Cascading)
                throw new NotImplementedException(string.Format("Mode {0} has not implemented Remove", Settings.Mode));
            bool flag = _handler.Remove(key);
            if (!EndOfChain)
                flag ^= _nextProvider.Remove(key);
            return flag;
        }

        public void Clear()
        {
            EnsureCacheProviderIsConfigured();
            CacheStatistics.Flush();
            if (Settings.Mode == ConfigurableCacheProviderMode.Fallback)
            {
                if (_handler.ProviderStatus == CacheProviderStatus.Ready)
                {
                    _handler.Clear();
                }
                else
                {
                    if (EndOfChain)
                        return;
                    _nextProvider.Clear();
                }
            }
            else
            {
                if (Settings.Mode != ConfigurableCacheProviderMode.Cascading)
                    throw new NotImplementedException(string.Format("Mode {0} has not implemented Clear", Settings.Mode));
                _handler.Clear();
                if (EndOfChain)
                    return;
                _nextProvider.Clear();
            }
        }

        private void EnsureCacheProviderIsConfigured()
        {
            if (_configured)
                return;
            Settings.Lock();
            BuildChain(new Queue<string>(Settings.Chain.Split(new char[2]
            {
        ',',
        ';'
            }).Select(c => c.Trim())));
            _configured = true;
        }

        private void BuildChain(Queue<string> providerChain)
        {
            while (providerChain.Any())
            {
                Func<Lazy<ICacheProvider, IDictionary<string, object>>, bool> provider = null;

                string str = providerChain.Dequeue();
                ICacheProvider cacheProvider = str == "Memory" ? MemoryCacheProvider.GetInstance(CacheName)
                            : DependencyInjector.AllWithMetaData<ICacheProvider>()
                            .Filter("CacheType", str).Resolve();

                if (cacheProvider != null)
                {
                    cacheProvider.CacheName = CacheName;
                    _handler = cacheProvider;
                    _nextProvider = new ConfigurableCacheProvider(CacheName, providerChain)
                    {
                        _configured = true
                    };
                }
            }
            if (_handler != null)
                return;
            _handler = MemoryCacheProvider.GetInstance(CacheName);
        }

        private static T InjectMiss<T>(Func<T> getFunc, out bool missed)
        {
            missed = true;
            CacheStatistics.Miss();
            return getFunc();
        }

        private bool EndOfChain => _nextProvider == null || _nextProvider._handler == null;
    }
}
