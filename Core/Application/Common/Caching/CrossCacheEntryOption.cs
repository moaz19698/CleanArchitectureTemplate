using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application.Common.Caching
{
    public class CrossCacheEntryOption
    {
        #region Properties
        public int SlidingExpiration { get; set; }
        public int AbsoluteExpiration { get; set; }
        public CacheItemPriority Priority { get; set; }
        public long LimitSize { get; set; }
        public bool IsEnabled { get; set; }
        public CacheStore CacheStore { get; set; }
        #endregion

        #region Mapping to Entry option
        public static implicit operator MemoryCacheEntryOptions?(CrossCacheEntryOption? options)
        {
            return options is not null ? new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(options.SlidingExpiration),
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(options.AbsoluteExpiration),
                Priority = options.Priority,
                Size = options.LimitSize,
            } : null;
        }
        public static implicit operator DistributedCacheEntryOptions(CrossCacheEntryOption? options)
        {
            return options is not null ? new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(options.SlidingExpiration),
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(options.AbsoluteExpiration),
            } : new DistributedCacheEntryOptions();
        }
        #endregion
    }
}
