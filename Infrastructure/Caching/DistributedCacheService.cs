using CleanArchitecture.Application.Common.Abstracts.Caching;
using CleanArchitecture.Common.Operation;
using CleanArchitecture.Infrastructure.Caching.RedisSetupConfigurationOptions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CleanArchitecture.Infrastructure.Caching;
public class DistributedCacheService : IDistributedCacheService
{
    #region Dependencies
    private readonly IDatabase _db;
    private readonly ILogger<DistributedCacheService> _logger;
    public IConnectionMultiplexer Connection { get; }
    #endregion

    #region Properties
    private RedisOptions RedisOptions { get; }
    private bool IsConnected
    {
        get
        {
            if (!Connection.IsConnected)
            {
                _logger.Log(LogLevel.Error, "Caching: Redis Connection is not avalilable can't connect to the server");
            }

            return Connection.IsConnected;
        }
    }

    #endregion

    #region Constructor
    public DistributedCacheService(IOptions<RedisOptions> options,
                                   IRedisConnection redisConnection,
                                   ILogger<DistributedCacheService> logger)
    {
        Connection = redisConnection.Connection;
        _logger = logger;
        _db = Connection.GetDatabase();

        _logger.LogInformation("Cachig:Redis-Configuration:{Connetion}", Connection.Configuration);
        RedisOptions = options.Value;
    }
    #endregion

    #region Methods
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (!IsConnected)
        {
            return null;
        }
        string? cachedValue = await _db.StringGetAsync(key);

        if (cachedValue == null)
        {
            return null;
        }
        T? value = JsonConvert.DeserializeObject<T>(cachedValue);
        _logger.LogInformation("Caching: Retrieved the cached value of key: {Key} from  Redis ", key);
        return value;
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
    {
        await SetAsync(key, value, new DistributedCacheEntryOptions(), cancellationToken);
    }

    public async Task SetAsync<T>(string key,
                                  T value,
                                  DistributedCacheEntryOptions options,
                                  CancellationToken cancellationToken = default) where T : class
    {
        if (IsConnected)
        {
            TimeSpan expiryTime = options.AbsoluteExpiration!.Value.DateTime.Subtract(DateTime.Now);
            string cacheValue = JsonConvert.SerializeObject(value);
            await _db.StringSetAsync(key, cacheValue, expiryTime);
            _logger.LogInformation("Caching: Added in the Redis Cache value with the Key:{Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (IsConnected)
        {
            _ = await _db.KeyDeleteAsync(key);
            _logger.LogInformation("Caching: Removed Cache value with the Key:{Key} from Redis", key);
        }
    }

    public async Task<int> RemoveKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        int count = 0;

        if (IsConnected && keys.Any())
        {
            IEnumerable<Task<bool>> removeTasks = keys.Select(key => _db.KeyDeleteAsync(key))
                                                      .ToList();
            var result = await Task.WhenAll(removeTasks);
            count = result.Count(r => r);
            _logger.LogInformation("Caching: {Count} Keys have been removed from Redis Cache Key:[{Keys}]", count, string.Join(',', keys));
        }

        return count;
    }

    public async Task<int> RemoveFromDistributedByKeyPattern(string keyPattern, CancellationToken cancellationToken)
    {
        int count = 0;

        if (IsConnected)
        {
            var server = Connection.GetServer($"{RedisOptions.Host}:{RedisOptions.Port}");
            var keys = server.Keys(pattern: keyPattern).Select(k => k.ToString()).ToList();
            count = await RemoveKeysAsync(keys, cancellationToken);
        }

        return count;
    }

    public async Task PublishMessageAsync(string channel, string keyPrefix)
    {
        Exception? ex = await OperationExecuter.RetryAsync(async () =>
        {
            var subscriber = Connection.GetSubscriber();
            await subscriber.PublishAsync(RedisChannel.Pattern(channel), keyPrefix);
            _logger.LogInformation("Caching: the key Prefix:{KeyPrefix} published as invalidate Key To redis Invaliate cache channel", keyPrefix);
        });

        if (ex is RedisConnectionException)
        {
            _logger.LogError(ex, "Caching: Redis Connection timeout with exception {Ex} ", ex);
        }
    }

    public async Task SubscribAsync(string channel, Action<string> action)
    {
        if (IsConnected)
        {
            var subscriber = Connection.GetSubscriber();
            await subscriber.SubscribeAsync(RedisChannel.Pattern(channel), (channel, value) =>
            {
                _logger.LogInformation("Caching: Subscribed on the Redis channel:{Channel}", channel);
                action(value!);
            });
        }
    }


    #endregion
}
