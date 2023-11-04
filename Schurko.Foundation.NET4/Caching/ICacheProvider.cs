
using System;
using System.Collections.Generic;


#nullable enable
namespace Schurko.Foundation.Caching
{
    public interface ICacheProvider
    {
        string CacheName { get; set; }

        string CacheDescription { get; set; }

        string CacheProviderDescription { get; }

        ICacheStatistics Statistics { get; }

        CacheProviderStatus ProviderStatus { get; }

        TObject Add<TObject>(
          string key,
          TObject item,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null);

        object Get(string key);

        IEnumerable<KeyValuePair<string, object>> GetByTag(string tag);

        IEnumerable<KeyValuePair<string, object>> GetByAnyTag(IEnumerable<string> tags);

        IEnumerable<KeyValuePair<string, object>> GetByAllTags(IEnumerable<string> tags);

        TObject GetOrAdd<TObject>(
          string key,
          Func<TObject> getItem,
          CacheExpiration expiration = null,
          IEnumerable<string> tags = null);

        bool Remove(string key);

        void Clear();
    }
}
