
using System;

namespace Schurko.Foundation.Caching.Memory
{
    public struct MemoryCacheItemExpiry
    {
        public DateTime AbsoluteExpiryTime { get; set; }

        public DateTime SlidingExpiryTime { get; set; }

        public TimeSpan SlidingInterval { get; set; }
    }
}
