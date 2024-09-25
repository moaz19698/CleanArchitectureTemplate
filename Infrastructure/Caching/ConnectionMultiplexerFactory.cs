using CleanArchitecture.Infrastructure.Caching.RedisSetupConfigurationOptions;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text;

namespace CleanArchitecture.Infrastructure.Caching;
public interface IRedisConnection
{
    IConnectionMultiplexer Connection { get; }
}
public sealed class RedisConnection : IRedisConnection, IDisposable
{
    private ConnectionMultiplexer? connection = null;
    public IConnectionMultiplexer Connection => connection!;

    public RedisConnection(IConfiguration configuration)
    {
        IntializeConnection(configuration);
    }

    private void IntializeConnection(IConfiguration configuration)
    {
        connection = ConnectionMultiplexer.ConnectAsync(GetRedisConnectionString(configuration)).Result;
    }
    private static string GetRedisConnectionString(IConfiguration configuration)
    {
        RedisOptions redisOptions = new();
        RedisOptionsSetup optionSetup = new(configuration);
        optionSetup.Configure(redisOptions);
        StringBuilder connectionStringBuilder = new();
        connectionStringBuilder.Append($"{redisOptions.Host}:{redisOptions.Port}");
        connectionStringBuilder.Append(",abortConnect=false");

        if (string.IsNullOrEmpty(redisOptions.User) && string.IsNullOrEmpty(redisOptions.Password))
        {
            return connectionStringBuilder.ToString();
        }
        connectionStringBuilder.Append($",user={redisOptions.User},password={redisOptions.Password}");

        return connectionStringBuilder.ToString();
    }
    public void Dispose()
    {
        connection?.Dispose();
    }
}