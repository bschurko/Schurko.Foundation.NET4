
using Schurko.Foundation.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Composition;


#nullable enable
namespace Schurko.Foundation.Caching.File
{
    [Export(typeof(ICacheProvider))]
    [ExportMetadata("CacheType", "File")]
    public class FileCacheProvider : ICacheProvider
    {
        private static readonly MemoryCacheStatistics _cacheStatistics = new MemoryCacheStatistics();

        public string CacheName { get; set; }

        public string CacheDescription { get; set; }

        public string CacheProviderDescription => "Provider which will store the objects to the file system.";

        public ICacheStatistics Statistics => _cacheStatistics;

        public CacheProviderStatus ProviderStatus => CacheProviderStatus.Disabled;

        public TObject Add<TObject>(
          string key,
          TObject item,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null)
        {
            throw new NotImplementedException();
        }

        public object Get(string key) => throw new NotImplementedException();

        public IEnumerable<KeyValuePair<string, object>> GetByTag(string tag) => throw new NotImplementedException();

        public IEnumerable<KeyValuePair<string, object>> GetByAnyTag(IEnumerable<string> tags) => throw new NotImplementedException();

        public IEnumerable<KeyValuePair<string, object>> GetByAllTags(IEnumerable<string> tags) => throw new NotImplementedException();

        public TObject GetOrAdd<TObject>(
          string key,
          Func<TObject> getItem,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();
    }
}
