
using Microsoft.Extensions.Logging;
using Schurko.Foundation.Extensions;
using Schurko.Foundation.Logging;
using System;
using System.Configuration;


#nullable enable
namespace Schurko.Foundation.Caching
{
    public abstract class CacheSettings
    {
        public static T GetCacheSetting<T>(string cacheName, string setting, T defaultValue)
        {
            string str = null;
            if (!string.IsNullOrEmpty(cacheName))
                str = ConfigurationManager.AppSettings["Cache.{0}.{1}".FormatString(cacheName, setting)];
            if (str == null)
                str = ConfigurationManager.AppSettings["Cache.{0}".FormatString(setting)];
            if (str == null)
                return defaultValue;
            if (!typeof(T).IsEnum)
                return (T)Convert.ChangeType(str, typeof(T));
            try
            {
                return (T)Enum.Parse(typeof(T), str, true);
            }
            catch (Exception ex)
            {
                Log.Logger.LogError("Could not parse cache setting from App.config.", ex, cacheName, setting, str);
                return defaultValue;
            }
        }

        public bool Locked { get; private set; }

        public void Lock() => Locked = true;
    }
}
