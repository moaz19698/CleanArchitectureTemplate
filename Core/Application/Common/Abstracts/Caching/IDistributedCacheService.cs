using Microsoft.Extensions.Caching.Distributed;

namespace CleanArchitecture.Application.Common.Abstracts.Caching;
public interface IDistributedCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task SetAsync<T>(string key,
                     T value,
                     DistributedCacheEntryOptions options,
                     CancellationToken cancellationToken = default) where T : class;

    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task<int> RemoveKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    Task<int> RemoveFromDistributedByKeyPattern(string keyPattern, CancellationToken cancellationToken);

    Task PublishMessageAsync(string channel, string keyPrefix);

    Task SubscribAsync(string channel, Action<string> action);

}
