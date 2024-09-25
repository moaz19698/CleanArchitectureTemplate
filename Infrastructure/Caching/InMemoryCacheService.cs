using CleanArchitecture.Application.Common.Abstracts.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace CleanArchitecture.Infrastructure.Caching;

public class InMemoryCacheService : IInMemoryCacheService
{
    private readonly ILogger<InMemoryCacheService> _logger;
    #region Dependencies
    private readonly IMemoryCache _cache;
    #endregion

    #region Properties
    public static ConcurrentDictionary<string, bool> CacheKeys { get; set; } = new();
    #endregion

    #region Constructor
    public InMemoryCacheService(ILogger<InMemoryCacheService> logger,
                                IMemoryCache memoryCache)
    {
        _logger = logger;
        _cache = memoryCache;
    }
    #endregion

    #region Methods
    public void Set<T>(string key, T value, MemoryCacheEntryOptions? options = default) where T : class
    {

        _cache.Set(key, value, options);
        CacheKeys.TryAdd(key, false);
        _logger.LogInformation("Caching: Added in the Memory Cache value with the Key:{Key}", key);
    }

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            _logger.LogInformation("Caching: Retrieved the cached value of key: {Key} from  Memory", key);
            return value;
        }
        else
        {
            return default;
        }
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        CacheKeys.TryRemove(key, out bool _);
        _logger.LogInformation("Caching: Removed Cache value with the Key:{Key} from Memory", key);

    }

    public int RemoveByPrefix(string keyPrefix)
    {
        var count = 0;
        var keys = CacheKeys.Keys.Where(k => k.StartsWith(keyPrefix))
                                 .ToList();
        count = keys.Count;
        keys.ForEach(key => Remove(key));

        return count;
    }
    #endregion
}
