using FluentValidation;
using SupplyChainManagement.Application.Behaviors;
using SupplyChainManagement.Application.Features.Products.Queries.GetAll;

namespace SupplyChainManagement.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsAndDoesNotCallNext()
    {
        var behavior = new ValidationBehavior<GetAllProductsQuery, string>([new GetAllProductsQueryValidator()]);
        var nextCalled = false;

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(new GetAllProductsQuery(0, 10), () => { nextCalled = true; return Task.FromResult("ok"); }, CancellationToken.None));

        Assert.False(nextCalled);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CallsNext()
    {
        var behavior = new ValidationBehavior<GetAllProductsQuery, string>([new GetAllProductsQueryValidator()]);

        var result = await behavior.Handle(new GetAllProductsQuery(1, 10), () => Task.FromResult("ok"), CancellationToken.None);

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_WithoutValidators_CallsNext()
    {
        var behavior = new ValidationBehavior<GetAllProductsQuery, string>([]);

        var result = await behavior.Handle(new GetAllProductsQuery(0, 0), () => Task.FromResult("ok"), CancellationToken.None);

        Assert.Equal("ok", result);
    }
}
