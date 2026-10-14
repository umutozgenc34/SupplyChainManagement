using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace SupplyChainManagement.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse>(ICacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private static readonly CacheableAttribute? Cacheable = typeof(TRequest).GetCustomAttribute<CacheableAttribute>();
    private static readonly InvalidatesCacheAttribute? Invalidates = typeof(TRequest).GetCustomAttribute<InvalidatesCacheAttribute>();
    private static readonly TimeSpan VersionExpiration = TimeSpan.FromDays(30);

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (Cacheable is not null)
            return await HandleCacheableAsync(request, next, cancellationToken);

        var response = await next();

        if (Invalidates is not null && response is ServiceResult { Success: true })
        {
            foreach (var group in Invalidates.Groups)
                await cacheService.SetAsync(VersionKey(group), Guid.NewGuid().ToString("N"), VersionExpiration, cancellationToken);
        }

        return response;
    }

    private async Task<TResponse> HandleCacheableAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var version = await cacheService.GetAsync<string>(VersionKey(Cacheable!.Group), cancellationToken) ?? "0";
        var key = $"{Cacheable.Group}:{version}:{typeof(TRequest).Name}:{JsonSerializer.Serialize(request)}";

        var cached = await cacheService.GetAsync<TResponse>(key, cancellationToken);
        if (cached is ServiceResult cachedResult)
        {
            cachedResult.Status = HttpStatusCode.OK;
            return cached;
        }

        var response = await next();

        if (response is ServiceResult { Success: true })
            await cacheService.SetAsync(key, response, Cacheable.Expiration, cancellationToken);

        return response;
    }

    private static string VersionKey(string group) => $"cache-version:{group}";
}
