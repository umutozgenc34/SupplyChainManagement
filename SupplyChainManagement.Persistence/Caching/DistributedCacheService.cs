using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SupplyChainManagement.Application.Services.Caching;
using System.Text.Json;

namespace SupplyChainManagement.Persistence.Caching;

public class DistributedCacheService(IDistributedCache cache, ILogger<DistributedCacheService> logger) : ICacheService
{
    private static readonly TimeSpan BreakDuration = TimeSpan.FromSeconds(30);
    private long _skipUntilTicks;

    private bool IsBroken => DateTime.UtcNow.Ticks < Interlocked.Read(ref _skipUntilTicks);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (IsBroken)
            return default;

        try
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Break(ex, key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        if (IsBroken)
            return;

        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
            await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Break(ex, key);
        }
    }

    private void Break(Exception exception, string key)
    {
        Interlocked.Exchange(ref _skipUntilTicks, DateTime.UtcNow.Add(BreakDuration).Ticks);
        logger.LogWarning(exception, "Cache unavailable for key {Key}, skipping cache for {Seconds} seconds", key, BreakDuration.TotalSeconds);
    }
}
