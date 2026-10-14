using SupplyChainManagement.Application.Features.Orders.Commands.Create;
using SupplyChainManagement.Application.Features.Orders.Commands.Update;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Features.Products.Commands.Create;
using SupplyChainManagement.Application.Features.Products.Queries.GetAll;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Create;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Tests.Validators;

public class ValidatorTests
{
    [Fact]
    public void CreateOrder_WithNoItems_IsInvalid()
    {
        var result = new CreateOrderCommandValidator().Validate(new CreateOrderCommand([]));

        Assert.Contains(result.Errors, e => e.ErrorMessage == "Order must contain at least one item.");
    }

    [Fact]
    public void CreateOrder_WithZeroQuantity_IsInvalid()
    {
        var command = new CreateOrderCommand([new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 0 }]);

        var result = new CreateOrderCommandValidator().Validate(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateOrder_WithUndefinedStatus_IsInvalid()
    {
        var command = new UpdateOrderCommand(Guid.NewGuid(), [new OrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1 }], (OrderStatus)9);

        var result = new UpdateOrderCommandValidator().Validate(command);

        Assert.Contains(result.Errors, e => e.ErrorMessage == "Invalid order status.");
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(-1, false)]
    public void CreateProduct_QuantityMustNotBeNegative(int quantity, bool expectedValid)
    {
        var result = new CreateProductCommandValidator().Validate(new CreateProductCommand("Bolt", quantity, 1m, 1));

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Theory]
    [InlineData("05550000000", true)]
    [InlineData("5550000000", false)]
    [InlineData("0555000000a", false)]
    public void CreateSupplier_PhoneMustBeElevenDigits(string phone, bool expectedValid)
    {
        var result = new CreateSupplierCommandValidator().Validate(new CreateSupplierCommand("Acme", "a@acme.com", phone));

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Theory]
    [InlineData(1, 10, true)]
    [InlineData(0, 10, false)]
    [InlineData(1, 0, false)]
    [InlineData(1, 101, false)]
    public void GetAllProducts_PagingMustBeInRange(int pageNumber, int pageSize, bool expectedValid)
    {
        var result = new GetAllProductsQueryValidator().Validate(new GetAllProductsQuery(pageNumber, pageSize));

        Assert.Equal(expectedValid, result.IsValid);
    }
}
