

#nullable enable


namespace Schurko.Foundation.Caching
{
    public class ConfigurableCacheSettings : CacheSettings
    {
        private ConfigurableCacheProviderMode _mode;
        private string _chain;

        internal string CacheName { get; private set; }

        public ConfigurableCacheProviderMode Mode
        {
            get => _mode;
            set
            {
                if (Locked)
                    return;
                _mode = value;
            }
        }

        public string Chain
        {
            get => _chain;
            set
            {
                if (Locked)
                    return;
                _chain = value;
            }
        }

        public ConfigurableCacheSettings(string cacheName = null)
        {
            if (Locked)
                return;
            CacheName = cacheName;
            Mode = GetCacheSetting(cacheName, "Configurable.Mode", ConfigurableCacheProviderMode.Fallback);
            Chain = GetCacheSetting(cacheName, "Configurable.Chain", "Memory");
        }
    }
}
