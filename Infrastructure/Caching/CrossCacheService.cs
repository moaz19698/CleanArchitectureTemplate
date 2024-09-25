using CleanArchitecture.Application.Common.Abstracts.Caching;
using CleanArchitecture.Application.Common.Caching;
using CleanArchitecture.Common.Operation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Caching;
public class CrossCacheService : ICrossCacheService
{
    #region Dependencies
    private readonly IInMemoryCacheService _memoryCacheService;
    private readonly IDistributedCacheService _distributedCacheService;
    private const string CHANNEL = "InvalidateCache";
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    private readonly ILogger<CrossCacheService> _logger;

    #endregion

    #region Properties
    private CrossCacheEntryOption DefaultCrossCacheEntryOption { get; set; }
    #endregion

    #region Constructor
    public CrossCacheService(ILogger<CrossCacheService> logger,
                             IInMemoryCacheService cacheService,
                             IDistributedCacheService distributedCacheService,
                             IOptions<CrossCacheEntryOption> options)
    {
        _logger = logger;
        _memoryCacheService = cacheService;
        _distributedCacheService = distributedCacheService;
        DefaultCrossCacheEntryOption = options.Value;
    }
    #endregion

    #region Methods
    public async Task<T?> GetCacheAsync<T>(string cacheKey,
                                           CacheStore cacheStore,
                                           CrossCacheEntryOption? options = default,
                                           CancellationToken cancellationToken = default) where T : class
    {
        T? value = null;

        switch (cacheStore)
        {
            case CacheStore.InMemory:
                value = _memoryCacheService.Get<T>(cacheKey);
                break;
            case CacheStore.Redis:
                value = await _distributedCacheService.GetAsync<T>(cacheKey!, cancellationToken);
                break;
            case CacheStore.All:
                value = _memoryCacheService.Get<T>(cacheKey);

                if (value is null)
                {
                    value = await _distributedCacheService.GetAsync<T>(cacheKey!, cancellationToken);

                    if (value is not null)
                    {
                        _memoryCacheService.Set(cacheKey, value, options);
                    }
                }
                break;
        }

        return value;
    }

    public async Task SetCacheAsync<T>(string cacheKey,
                                       T value,
                                       CacheStore cacheStore,
                                       CrossCacheEntryOption? options = default,
                                       CancellationToken cancellationToken = default) where T : class
    {
        switch (cacheStore)
        {
            case CacheStore.InMemory:
                _memoryCacheService.Set(cacheKey, value, options);
                break;
            case CacheStore.Redis:
                await _distributedCacheService.SetAsync(cacheKey, value, options, cancellationToken);
                break;
            case CacheStore.All:
                _memoryCacheService.Set(cacheKey, value, options);
                await _distributedCacheService.SetAsync(cacheKey, value, options, cancellationToken);
                break;
        }
    }

    public async Task<T> GetOrCreateCacheAsync<T>(string cacheKey,
                                                  Func<Task<T>> getResonse,
                                                  CrossCacheEntryOption crossCacheEntryOption,
                                                  CacheStore cacheStore,
                                                  CancellationToken cancellationToken = default) where T : class, new()
    {
        T? response = null;

        if (!DefaultCrossCacheEntryOption.IsEnabled)
        {
            return await getResonse() ?? new();
        }

        response = await Semaphore.WaitThenReleaseAsync(async () =>
        {
            response = await GetCacheAsync<T>(cacheKey,
                                              cacheStore,
                                              crossCacheEntryOption,
                                              cancellationToken);

            if (response is null)
            {
                _logger.LogInformation("Caching: {CacheKey} not found in cache. Fetching from database.", cacheKey);
                response = await getResonse();

                if (response != default)
                {
                    await SetCacheAsync(cacheKey,
                                        response,
                                        cacheStore,
                                        crossCacheEntryOption,
                                        cancellationToken);
                }
            }
            else
            {
                _logger.LogInformation("Caching: {CacheKey} found in cache.", cacheKey);
            }

            return response ?? new();
        },
        cancellationToken);

        return response;
    }
    public async Task<T> GetOrCreateCacheAsync<T>(string cacheKey,
                                                  Func<Task<T>> getResonse,
                                                  CancellationToken cancellationToken = default) where T : class, new()
    {
        return await GetOrCreateCacheAsync(cacheKey,
                                           getResonse,
                                           DefaultCrossCacheEntryOption,
                                           DefaultCrossCacheEntryOption.CacheStore,
                                           cancellationToken);
    }
    public async Task RemoveAsync(string key, CacheStore cacheStore, CancellationToken cancellationToken = default)
    {
        await Semaphore.WaitThenReleaseAsync(async () =>
        {
            switch (cacheStore)
            {
                case CacheStore.InMemory:
                    _memoryCacheService.Remove(key);
                    break;
                case CacheStore.Redis:
                    await _distributedCacheService.RemoveAsync(key, cancellationToken);
                    break;
                case CacheStore.All:
                    _memoryCacheService.Remove(key);
                    await _distributedCacheService.RemoveAsync(key, cancellationToken);
                    await _distributedCacheService.PublishMessageAsync(CHANNEL, key.Split(':')[0]);
                    break;
            }
        },
        cancellationToken);
    }

    public async Task<int> InvalidateCacheAsync(string keyPrefix,
                                           CacheStore cacheStore,
                                           CancellationToken cancellationToken = default)
    {
        var count = 0;
        await Semaphore.WaitThenReleaseAsync(async () =>
        {
            switch (cacheStore)
            {
                case CacheStore.InMemory:
                    count = _memoryCacheService.RemoveByPrefix(keyPrefix);
                    break;
                case CacheStore.Redis:
                    count = await _distributedCacheService.RemoveFromDistributedByKeyPattern($"{keyPrefix}*", cancellationToken);
                    break;
                case CacheStore.All:
                    _ = _memoryCacheService.RemoveByPrefix(keyPrefix);
                    count = await _distributedCacheService.RemoveFromDistributedByKeyPattern($"{keyPrefix}*", cancellationToken);
                    await _distributedCacheService.PublishMessageAsync(CHANNEL, keyPrefix);
                    break;
            }
            _logger.LogInformation("Caching: invalidated {Count} cache keys the started with the key prefix {KeyPrefix}", count, keyPrefix);
        },
        cancellationToken);

        return count;
    }

    public async Task InvalidateLocalCacheBySubscibeInvalidateChannel()
    {
        var count = 0;

        await _distributedCacheService.SubscribAsync(CHANNEL, (keyPrefix) =>
        {
            count = _memoryCacheService.RemoveByPrefix(keyPrefix!);
            _logger.LogInformation("Caching: {Count} reomved the key prefix: {KeyPrefix} from local memory cache", count, keyPrefix);
        });
    }


    #endregion
}
