
using System;


#nullable enable
namespace Schurko.Foundation.Caching
{
    public class SlidingExpiration
    {
        private readonly CacheExpiration _currentExpiration;

        public SlidingExpiration(CacheExpiration currentExpiration) => _currentExpiration = currentExpiration ?? new CacheExpiration();

        public CacheExpiration None() => FromTimeSpan(TimeSpan.MaxValue);

        public CacheExpiration AddDays(double days) => FromTimeSpan(TimeSpan.FromDays(days));

        public CacheExpiration AddHours(double hours) => FromTimeSpan(TimeSpan.FromHours(hours));

        public CacheExpiration AddMinutes(double minutes) => FromTimeSpan(TimeSpan.FromMinutes(minutes));

        public CacheExpiration AddSeconds(double seconds) => FromTimeSpan(TimeSpan.FromSeconds(seconds));

        public CacheExpiration AddMilliseconds(double milliseconds) => FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds));

        public CacheExpiration FromTimeSpan(TimeSpan timeSpan)
        {
            _currentExpiration.SlidingInterval = _currentExpiration.SlidingInterval.Add(timeSpan);
            _currentExpiration.UseDefaultSlidingInterval = false;
            return _currentExpiration;
        }
    }
}
