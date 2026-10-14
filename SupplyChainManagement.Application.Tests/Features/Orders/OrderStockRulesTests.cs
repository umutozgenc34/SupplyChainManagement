using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Features.Orders.Rules;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Tests.Features.Orders;

public class OrderStockRulesTests
{
    [Fact]
    public void ReserveStock_WithEnoughStock_ReservesStockAndUsesProductPrice()
    {
        var product = TestData.Product(quantity: 10, price: 7m);
        var products = new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product };
        var requested = new List<OrderItemDto> { new() { ProductId = product.Id, Quantity = 3, Price = 0.01m } };

        var result = OrderStockRules.ReserveStock(requested, products);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(7m, item.Price);
        Assert.Equal(7, product.Quantity);
    }

    [Fact]
    public void ReserveStock_WithDuplicateProducts_MergesIntoSingleItem()
    {
        var product = TestData.Product(quantity: 10);
        var products = new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product };
        var requested = new List<OrderItemDto>
        {
            new() { ProductId = product.Id, Quantity = 2 },
            new() { ProductId = product.Id, Quantity = 3 }
        };

        var result = OrderStockRules.ReserveStock(requested, products);

        var item = Assert.Single(result.Data!);
        Assert.Equal(5, item.Quantity);
        Assert.Equal(5, product.Quantity);
    }

    [Fact]
    public void ReserveStock_WithInsufficientStock_FailsAndKeepsStock()
    {
        var product = TestData.Product(quantity: 2);
        var products = new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product };
        var requested = new List<OrderItemDto> { new() { ProductId = product.Id, Quantity = 3 } };

        var result = OrderStockRules.ReserveStock(requested, products);

        Assert.False(result.Success);
        Assert.Contains("Insufficient stock", result.Message);
        Assert.Equal(2, product.Quantity);
    }

    [Fact]
    public void ReserveStock_WithUnknownProduct_Fails()
    {
        var requested = new List<OrderItemDto> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } };

        var result = OrderStockRules.ReserveStock(requested, new Dictionary<Guid, Domain.Products.Product>());

        Assert.False(result.Success);
        Assert.Contains("not found", result.Message);
    }

    [Fact]
    public void ReleaseStock_AddsQuantitiesBackToProducts()
    {
        var product = TestData.Product(quantity: 4);
        var order = TestData.Order(OrderStatus.Pending, (product, 3));

        OrderStockRules.ReleaseStock(order.OrderItems, new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product });

        Assert.Equal(7, product.Quantity);
    }
}
