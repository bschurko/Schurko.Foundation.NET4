
using System;
using System.Threading;

namespace Schurko.Foundation.Caching.Memory
{
    public class MemoryCacheStatistics : ICacheStatistics
    {
        private DateTime _startDate;
        private long _items;
        private long _hits;
        private long _misses;
        private long _flushes;

        public DateTime StartDate => _startDate;

        public long Items => Interlocked.Read(ref _items);

        public long Hits => Interlocked.Read(ref _hits);

        public long Misses => Interlocked.Read(ref _misses);

        public long Flushes => Interlocked.Read(ref _flushes);

        public void Reset()
        {
            _startDate = DateTime.UtcNow;
            Interlocked.Exchange(ref _hits, 0L);
            Interlocked.Exchange(ref _misses, 0L);
            Interlocked.Exchange(ref _flushes, 0L);
        }

        public MemoryCacheStatistics() => _startDate = DateTime.UtcNow;

        public void SetItemCount(long count) => Interlocked.Exchange(ref _items, count);

        public void AddItem() => Interlocked.Increment(ref _items);

        public void RemoveItem() => Interlocked.Decrement(ref _items);

        public void Hit() => Interlocked.Increment(ref _hits);

        public void Miss() => Interlocked.Increment(ref _misses);

        public void Flush() => Interlocked.Increment(ref _flushes);
    }
}
