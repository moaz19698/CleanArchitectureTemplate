using CleanArchitecture.Application.Common.Abstracts.Caching;
using CleanArchitecture.Application.Common.Caching;
using CleanArchitecture.Application.Common.Extensions;
using CleanArchitecture.Application.Common.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;


namespace CleanArchitecture.Application.Common.Behaviours;

public class CachingBehaviour<TRequest, TResponse> : IRequestResponsePipeline<TRequest, TResponse>
        where TRequest : ICacheable

{
    #region Dependency
    private readonly ICrossCacheService _crossCacheService;
    private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger;
    private CrossCacheEntryOption DefaultCrossCacheEntryOption { get; set; }
    #endregion

    #region Constructor
    public CachingBehaviour(ICrossCacheService crossCache,
                            ILogger<CachingBehaviour<TRequest, TResponse>> logger,
                            IOptions<CrossCacheEntryOption> options)
    {
        _crossCacheService = crossCache;
        _logger = logger;
        DefaultCrossCacheEntryOption = options.Value;

    }
    #endregion

    #region Handle
    public async Task<Response<TResponse>> Handle(TRequest request,
                                                  MyRequestResponseHandlerDelegate<TResponse> next,
                                                  CancellationToken cancellationToken)
    {
        _logger.LogInformation("Caching Behaviour started");
        var cacheAttribute = request.GetType().GetCustomAttribute<CacheAttribute>();

        if (!DefaultCrossCacheEntryOption.IsEnabled || cacheAttribute is null)
        {
            _logger.LogInformation("Caching is disabled");
            _logger.LogInformation("Caching Behaviour Ended");

            return await next();
        }

        var cacheKey = $"{cacheAttribute.KeyPrefix}:{request.GetType().Name}:{request.CahcheKeyIdentifire}";
        var crossCacheEntryOption = cacheAttribute.ToCrossCacheEntryOptionsOrDefault(DefaultCrossCacheEntryOption);
        Response<TResponse>? response;

        try
        {
            if (cacheAttribute.ToInvalidate)
            {
                response = await next();
                await _crossCacheService.InvalidateCacheAsync($"{cacheAttribute.KeyPrefix}:",
                                                              crossCacheEntryOption.CacheStore,
                                                              cancellationToken);
                _logger.LogInformation("{KeyPrefix} become invaild in cache. will remove with all related keys from cache.",
                                       cacheAttribute.KeyPrefix);
            }
            else
            {
                response = await _crossCacheService.GetOrCreateCacheAsync(cacheKey,
                                                                          () => next(),
                                                                          crossCacheEntryOption,
                                                                          crossCacheEntryOption.CacheStore,
                                                                          cancellationToken);
            }
            _logger.LogInformation("Cach Behaviour Ended");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "The caching operation failed, there are exception occurred with {Message}",
                                   ex.Message);
            _logger.LogInformation("Caching Behaviour Ended");

            return await next();
        }
    }
    #endregion
}
