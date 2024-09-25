using CleanArchitecture.Application.Common.Abstracts.Caching;
using CleanArchitecture.Infrastructure.Caching;
using CleanArchitecture.Infrastructure.Caching.RedisSetupConfigurationOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace CleanArchitecture.WebAPI.Configuration
{
    public class CachingServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRedisConnection, RedisConnection>();
            services.ConfigureOptions<RedisOptionsSetup>();
            services.ConfigureOptions<RedisConfigurationOptionsSetup>();
            services.ConfigureOptions<CrossCacheEntryOptionSetup>();

            services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();
            services.AddSingleton<IDistributedCacheService, DistributedCacheService>();
            services.AddSingleton<ICrossCacheService, CrossCacheService>();
            services.AddSingleton<IHostedService, InvalidationCacheChannelSubscriber>();
        }
    }
}
