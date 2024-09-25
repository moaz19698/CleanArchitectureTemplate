using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application.Common.Abstracts.Caching
{
    public interface IInMemoryCacheService
    {
        void Set<T>(string key, T value, MemoryCacheEntryOptions? options = default) where T : class;

        T? Get<T>(string key);

        void Remove(string key);

        int RemoveByPrefix(string keyPrefix);
    }
}
