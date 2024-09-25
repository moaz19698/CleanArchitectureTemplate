using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application.Common.Caching
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CacheAttribute : Attribute
    {
        #region Properties
        public required string KeyPrefix { get; set; }

        public bool ToInvalidate { get; set; }

        public string? SlidingExpirationMinutes { get; set; }

        public string? AbsoluteExpirationMinutes { get; set; }

        public CacheItemPriority Priority { get; set; }

        public string? LimitSize { get; set; }

        public CacheStore CacheStore { get; set; }
        #endregion
    }

}
