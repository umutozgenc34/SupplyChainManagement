using AutoMapper;
using SupplyChainManagement.Application;
using SupplyChainManagement.Domain.Orders;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Application.Tests;

public static class TestData
{
    public static IMapper Mapper { get; } =
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(ApplicationAssembly).Assembly)).CreateMapper();

    public static Product Product(int quantity = 10, decimal price = 5m, string name = "Test Product") =>
        new() { Id = Guid.NewGuid(), Name = name, Quantity = quantity, Price = price, SupplierId = 1 };

    public static Order Order(OrderStatus status, params (Product Product, int Quantity)[] items) =>
        new()
        {
            Id = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            Status = status,
            OrderItems = items.Select(i => new OrderItem(i.Product.Id, i.Quantity, i.Product.Price)).ToList()
        };
}
