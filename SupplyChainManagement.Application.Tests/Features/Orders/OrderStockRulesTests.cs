using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Features.Orders.Rules;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Tests.Features.Orders;

public class OrderStockRulesTests
{
    [Fact]
    public void CreateOrderItems_WithEnoughStock_ReservesStockAndUsesProductPrice()
    {
        var product = TestData.Product(quantity: 10, price: 7m);
        var products = new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product };
        var requested = new List<OrderItemDto> { new() { ProductId = product.Id, Quantity = 3, Price = 0.01m } };

        var result = OrderStockRules.CreateOrderItems(requested, products, reserveStock: true);

        Assert.True(result.Success);
        var item = Assert.Single(result.Data!);
        Assert.Equal(3, item.Quantity);
        Assert.Equal(7m, item.Price);
        Assert.Equal(7, product.Quantity);
    }

    [Fact]
    public void CreateOrderItems_WithDuplicateProducts_MergesIntoSingleItem()
    {
        var product = TestData.Product(quantity: 10);
        var products = new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product };
        var requested = new List<OrderItemDto>
        {
            new() { ProductId = product.Id, Quantity = 2 },
            new() { ProductId = product.Id, Quantity = 3 }
        };

        var result = OrderStockRules.CreateOrderItems(requested, products, reserveStock: true);

        var item = Assert.Single(result.Data!);
        Assert.Equal(5, item.Quantity);
        Assert.Equal(5, product.Quantity);
    }

    [Fact]
    public void CreateOrderItems_WithInsufficientStock_FailsAndKeepsStock()
    {
        var product = TestData.Product(quantity: 2);
        var products = new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product };
        var requested = new List<OrderItemDto> { new() { ProductId = product.Id, Quantity = 3 } };

        var result = OrderStockRules.CreateOrderItems(requested, products, reserveStock: true);

        Assert.False(result.Success);
        Assert.Contains("Insufficient stock", result.Message);
        Assert.Equal(2, product.Quantity);
    }

    [Fact]
    public void CreateOrderItems_WithUnknownProduct_Fails()
    {
        var requested = new List<OrderItemDto> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } };

        var result = OrderStockRules.CreateOrderItems(requested, new Dictionary<Guid, Domain.Products.Product>(), reserveStock: true);

        Assert.False(result.Success);
        Assert.Contains("not found", result.Message);
    }

    [Fact]
    public void CreateOrderItems_WithoutReservation_DoesNotCheckOrChangeStock()
    {
        var product = TestData.Product(quantity: 1);
        var products = new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product };
        var requested = new List<OrderItemDto> { new() { ProductId = product.Id, Quantity = 5 } };

        var result = OrderStockRules.CreateOrderItems(requested, products, reserveStock: false);

        Assert.True(result.Success);
        Assert.Equal(1, product.Quantity);
    }

    [Fact]
    public void ReleaseStock_AddsQuantitiesBackToProducts()
    {
        var product = TestData.Product(quantity: 4);
        var order = TestData.Order(OrderStatus.Pending, (product, 3));

        OrderStockRules.ReleaseStock(order.OrderItems, new Dictionary<Guid, Domain.Products.Product> { [product.Id] = product });

        Assert.Equal(7, product.Quantity);
    }

    [Theory]
    [InlineData(OrderStatus.Pending, true)]
    [InlineData(OrderStatus.Delivered, true)]
    [InlineData(OrderStatus.Canceled, false)]
    public void HoldsStock_ReturnsFalseOnlyForCanceledOrders(OrderStatus status, bool expected)
    {
        Assert.Equal(expected, OrderStockRules.HoldsStock(status));
    }
}
