


namespace Schurko.Foundation.Caching
{
    public static class Expiration
    {
        public static SlidingExpiration Sliding => new SlidingExpiration(null);

        public static AbsoluteExpiration Absolute => new AbsoluteExpiration(null);

        public static CacheExpiration Default(string name = "") => new CacheExpiration()
        {
            Name = name,
            UseDefaultExpiration = true,
            UseDefaultSlidingInterval = true
        };
    }
}
