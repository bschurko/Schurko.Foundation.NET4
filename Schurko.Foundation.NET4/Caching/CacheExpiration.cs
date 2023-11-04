
using System;


#nullable enable
namespace Schurko.Foundation.Caching
{
    public class CacheExpiration
    {
        public string Name { get; internal set; }

        public bool UseDefaultSlidingInterval { get; internal set; }

        public TimeSpan SlidingInterval { get; internal set; }

        public bool UseDefaultExpiration { get; internal set; }

        public TimeSpan ExpirationInterval { get; internal set; }

        internal CacheExpiration()
        {
            UseDefaultExpiration = true;
            UseDefaultSlidingInterval = true;
        }

        public SlidingExpiration Sliding => new SlidingExpiration(this);

        public AbsoluteExpiration Absolute => new AbsoluteExpiration(this);
    }
}
