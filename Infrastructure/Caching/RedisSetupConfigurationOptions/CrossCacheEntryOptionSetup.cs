using CleanArchitecture.Application.Common.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Caching.RedisSetupConfigurationOptions;

public class CrossCacheEntryOptionSetup : IConfigureOptions<CrossCacheEntryOption>
{
    private const string SECTION_NAME = "CrossCacheEntry";

    #region Dependencies
    private IConfiguration Configuration { get; }
    #endregion

    #region Constructor
    public CrossCacheEntryOptionSetup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    #endregion

    #region Configure
    public void Configure(CrossCacheEntryOption options)
    {
        var section = Configuration.GetSection(SECTION_NAME);

        if (section is not null)
        {
            options.SlidingExpiration = section.GetValue<int>(nameof(options.SlidingExpiration));
            options.AbsoluteExpiration = section.GetValue<int>(nameof(options.AbsoluteExpiration));
            options.LimitSize = section.GetValue<long>(nameof(options.LimitSize));
            options.IsEnabled = section.GetValue<bool>(nameof(options.IsEnabled));
            options.CacheStore = section.GetValue<CacheStore>(nameof(CacheStore));
        }
    }
    #endregion
}
