
using System;

namespace Schurko.Foundation.Caching
{
    public interface ICacheStatistics
    {
        DateTime StartDate { get; }

        long Items { get; }

        long Hits { get; }

        long Misses { get; }

        long Flushes { get; }

        void Reset();
    }
}
