using CleanArchitecture.Application.Common.Caching;

namespace CleanArchitecture.Application.Common.Extensions
{
    public static class CacheAttributeExtension
    {
        public static CrossCacheEntryOption ToCrossCacheEntryOptionsOrDefault(this CacheAttribute attribute,
                                                                              CrossCacheEntryOption defaultOptions)
        {
            return new CrossCacheEntryOption
            {
                SlidingExpiration = int.TryParse(attribute.SlidingExpirationMinutes, out int slidingEpiration)
                                                    ? slidingEpiration
                                                    : defaultOptions.SlidingExpiration,
                AbsoluteExpiration = int.TryParse(attribute.AbsoluteExpirationMinutes, out int absoluteExpiration)
                                                    ? absoluteExpiration
                                                    : defaultOptions.AbsoluteExpiration,
                Priority = attribute.Priority,
                LimitSize = long.TryParse(attribute.LimitSize, out long limit) ? limit : defaultOptions.LimitSize,
                CacheStore = attribute.CacheStore is default(CacheStore)
                                                ? defaultOptions.CacheStore
                                                : attribute.CacheStore
            };
        }
    }
}
