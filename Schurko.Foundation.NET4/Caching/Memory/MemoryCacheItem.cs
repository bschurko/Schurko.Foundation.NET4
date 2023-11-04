
using System.Collections.Generic;


#nullable enable
namespace Schurko.Foundation.Caching.Memory
{
    public class MemoryCacheItem
    {
        public object Item { get; set; }

        public MemoryCacheItemExpiry Expiry { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
