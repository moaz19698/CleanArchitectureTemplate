using CleanArchitecture.Application.Common.Abstracts.Caching;
using CleanArchitecture.Application.Common.Abstracts.Persistence;
using CleanArchitecture.Infrastructure.Caching;
using CleanArchitecture.Infrastructure.Caching.RedisSetupConfigurationOptions;
using CleanArchitecture.Persistence.EF;
using CleanArchitecture.Persistence.EF.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArchitecture.WebAPI.Configuration
{
    public class InfrastructureServicesInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>((sp, options) =>
               options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
               .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()));
            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
            services.ConfigureOptions<RedisOptionsSetup>();


            services.AddStackExchangeRedisCache(ConfigureRedis(configuration));
            services.ConfigureOptions<RedisConfigurationOptionsSetup>();

            services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();
            services.AddSingleton<IDistributedCacheService, DistributedCacheService>();
            services.AddSingleton<ICrossCacheService, CrossCacheService>();
            services.AddSingleton<IHostedService, InvalidationCacheChannelSubscriber>();
            services.AddScoped<ApplicationDbContextInitializer>();
        }
        private static Action<RedisCacheOptions> ConfigureRedis(IConfiguration configuration)
        {
            return option =>
            {
                RedisOptions redisOPtions = new();
                RedisOptionsSetup optionSetup = new(configuration);
                optionSetup.Configure(redisOPtions);

                option.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    EndPoints = { { redisOPtions.Host ?? throw new InvalidOperationException("Redis Host can't be null"), redisOPtions.Port } },
                    User = redisOPtions.User,
                    Password = redisOPtions.Password,
                    Ssl = redisOPtions.Ssl
                };
            };
        }
    }
}
