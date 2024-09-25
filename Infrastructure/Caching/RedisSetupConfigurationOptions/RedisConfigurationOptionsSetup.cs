using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CleanArchitecture.Infrastructure.Caching.RedisSetupConfigurationOptions;
public class RedisConfigurationOptionsSetup : IConfigureNamedOptions<ConfigurationOptions>
{
    #region Properties
    public RedisOptions redisOptions { get; }
    #endregion

    #region Constructor
    public RedisConfigurationOptionsSetup(IOptions<RedisOptions> options)
    {
        redisOptions = options.Value;
    }
    #endregion

    #region Configure
    public void Configure(string? name, ConfigurationOptions options)
    {
        options.EndPoints.Add(redisOptions.Host!, redisOptions.Port);
        options.User = redisOptions.User;
        options.Password = redisOptions.Password;
        options.Ssl = redisOptions.Ssl;
    }

    public void Configure(ConfigurationOptions options)
    {
        options.EndPoints.Add(redisOptions.Host!, redisOptions.Port);
        options.User = redisOptions.User;
        options.Password = redisOptions.Password;
        options.Ssl = redisOptions.Ssl;
    }
    #endregion
}
