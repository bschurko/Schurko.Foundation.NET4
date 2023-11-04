
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Caching
{
    public interface IQueryableCacheProvider
    {
        IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query);

        IEnumerable<T> GetOrCreateCache<T>(IQueryable<T> query, TimeSpan cacheDuration);

        bool RemoveFromCache<T>(IQueryable<T> query);
    }
}
