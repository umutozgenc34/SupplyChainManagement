using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Behaviors;
using SupplyChainManagement.Application.Services.Caching;
using System.Net;
using System.Text.Json;

namespace SupplyChainManagement.Application.Tests.Behaviors;

public class CachingBehaviorTests
{
    [Cacheable("items")]
    public record CachedQuery(int Page) : IRequest<ServiceResult<string>>;

    [InvalidatesCache("items")]
    public record InvalidatingCommand : IRequest<ServiceResult>;

    public record PlainCommand : IRequest<ServiceResult>;

    private sealed class InMemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, string> _store = new();

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) =>
            Task.FromResult(_store.TryGetValue(key, out var json) ? JsonSerializer.Deserialize<T>(json) : default);

        public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            _store[key] = JsonSerializer.Serialize(value);
            return Task.CompletedTask;
        }

        public int Count => _store.Count;
    }

    private readonly InMemoryCacheService _cache = new();

    private Task<ServiceResult<string>> Query(int page, Func<ServiceResult<string>> handler) =>
        new CachingBehavior<CachedQuery, ServiceResult<string>>(_cache)
            .Handle(new CachedQuery(page), () => Task.FromResult(handler()), CancellationToken.None);

    private Task<ServiceResult> Send<TCommand>(TCommand command, ServiceResult result) where TCommand : notnull =>
        new CachingBehavior<TCommand, ServiceResult>(_cache)
            .Handle(command, () => Task.FromResult(result), CancellationToken.None);

    [Fact]
    public async Task CacheableQuery_SecondCall_IsServedFromCacheWithOkStatus()
    {
        var calls = 0;
        ServiceResult<string> Handler() { calls++; return ServiceResult<string>.SuccessResult("data"); }

        await Query(1, Handler);
        var second = await Query(1, Handler);

        Assert.Equal(1, calls);
        Assert.Equal("data", second.Data);
        Assert.Equal(HttpStatusCode.OK, second.Status);
    }

    [Fact]
    public async Task CacheableQuery_DifferentParameters_UseDifferentEntries()
    {
        var calls = 0;
        ServiceResult<string> Handler() { calls++; return ServiceResult<string>.SuccessResult("data"); }

        await Query(1, Handler);
        await Query(2, Handler);

        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task CacheableQuery_FailedResult_IsNotCached()
    {
        var calls = 0;
        ServiceResult<string> Handler() { calls++; return ServiceResult<string>.Failure("missing", HttpStatusCode.NotFound); }

        await Query(1, Handler);
        var second = await Query(1, Handler);

        Assert.Equal(2, calls);
        Assert.Equal(HttpStatusCode.NotFound, second.Status);
    }

    [Fact]
    public async Task InvalidatingCommand_OnSuccess_ForcesQueryToReload()
    {
        var calls = 0;
        ServiceResult<string> Handler() { calls++; return ServiceResult<string>.SuccessResult($"v{calls}"); }

        await Query(1, Handler);
        await Send(new InvalidatingCommand(), ServiceResult.SuccessResult());
        var afterInvalidation = await Query(1, Handler);

        Assert.Equal(2, calls);
        Assert.Equal("v2", afterInvalidation.Data);
    }

    [Fact]
    public async Task InvalidatingCommand_OnFailure_KeepsCache()
    {
        var calls = 0;
        ServiceResult<string> Handler() { calls++; return ServiceResult<string>.SuccessResult("data"); }

        await Query(1, Handler);
        await Send(new InvalidatingCommand(), ServiceResult.Failure("nope"));
        await Query(1, Handler);

        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task PlainCommand_DoesNotTouchCache()
    {
        await Send(new PlainCommand(), ServiceResult.SuccessResult());

        Assert.Equal(0, _cache.Count);
    }
}
