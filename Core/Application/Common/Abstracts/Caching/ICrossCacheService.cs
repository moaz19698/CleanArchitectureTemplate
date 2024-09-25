using CleanArchitecture.Application.Common.Caching;

namespace CleanArchitecture.Application.Common.Abstracts.Caching;
public interface ICrossCacheService
{
    Task<T?> GetCacheAsync<T>(string cacheKey,
                              CacheStore cacheStore,
                              CrossCacheEntryOption? options = default,
                              CancellationToken cancellationToken = default) where T : class;

    Task SetCacheAsync<T>(string cacheKey,
                          T value,
                          CacheStore cacheStore,
                          CrossCacheEntryOption? options = default,
                          CancellationToken cancellationToken = default) where T : class;

    Task<T> GetOrCreateCacheAsync<T>(string cacheKey,
                                      Func<Task<T>> getResonse,
                                      CrossCacheEntryOption crossCacheEntryOption,
                                      CacheStore cacheStore,
                                      CancellationToken cancellationToken = default) where T : class, new();

    Task<T> GetOrCreateCacheAsync<T>(string cacheKey,
                                                  Func<Task<T>> getResonse,
                                                  CancellationToken cancellationToken = default) where T : class, new();

    Task RemoveAsync(string key, CacheStore cacheStore, CancellationToken cancellationToken = default);

    Task<int> InvalidateCacheAsync(string keyPrefix, CacheStore cacheStore, CancellationToken cancellationToken = default);

    Task InvalidateLocalCacheBySubscibeInvalidateChannel();
}
