using Moq;
using SupplyChainManagement.Application.Features.Orders.Commands.Create;
using SupplyChainManagement.Application.Features.Orders.Commands.Delete;
using SupplyChainManagement.Application.Features.Orders.Commands.Update;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;
using SupplyChainManagement.Domain.Products;
using System.Net;

namespace SupplyChainManagement.Application.Tests.Features.Orders;

public class OrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private void SetupProducts(params Product[] products) =>
        _productRepository
            .Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products.ToList());

    private void SetupOrder(Order order) =>
        _orderRepository
            .Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

    private static List<OrderItemDto> Items(Product product, int quantity) =>
        [new() { ProductId = product.Id, Quantity = quantity }];

    [Fact]
    public async Task Create_WithEnoughStock_SavesPendingOrderAndReservesStock()
    {
        var product = TestData.Product(quantity: 10, price: 4m);
        SetupProducts(product);
        var handler = new CreateOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new CreateOrderCommand(Items(product, 3)), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(OrderStatus.Pending, result.Data!.Status);
        Assert.Equal(12m, result.Data.TotalPrice);
        Assert.Equal(7, product.Quantity);
        _orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithInsufficientStock_ReturnsBadRequestAndDoesNotSave()
    {
        var product = TestData.Product(quantity: 2);
        SetupProducts(product);
        var handler = new CreateOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new CreateOrderCommand(Items(product, 3)), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.BadRequest, result.Status);
        _orderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenOrderMissing_ReturnsNotFound()
    {
        var handler = new UpdateOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new UpdateOrderCommand(Guid.NewGuid(), [], OrderStatus.Pending), CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.Status);
    }

    [Fact]
    public async Task Update_ChangingQuantity_ReleasesOldReservationAndReservesNewOne()
    {
        var product = TestData.Product(quantity: 7);
        var order = TestData.Order(OrderStatus.Pending, (product, 3));
        SetupOrder(order);
        SetupProducts(product);
        var handler = new UpdateOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new UpdateOrderCommand(order.Id, Items(product, 5), OrderStatus.Pending), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(5, product.Quantity);
        Assert.Equal(5, Assert.Single(order.OrderItems).Quantity);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_ToCanceled_ReleasesStock()
    {
        var product = TestData.Product(quantity: 7);
        var order = TestData.Order(OrderStatus.Pending, (product, 3));
        SetupOrder(order);
        SetupProducts(product);
        var handler = new UpdateOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, TestData.Mapper);

        await handler.Handle(new UpdateOrderCommand(order.Id, Items(product, 3), OrderStatus.Canceled), CancellationToken.None);

        Assert.Equal(10, product.Quantity);
        Assert.Equal(OrderStatus.Canceled, order.Status);
    }

    [Fact]
    public async Task Update_FromCanceledToPending_ReservesStockAgain()
    {
        var product = TestData.Product(quantity: 10);
        var order = TestData.Order(OrderStatus.Canceled, (product, 3));
        SetupOrder(order);
        SetupProducts(product);
        var handler = new UpdateOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, TestData.Mapper);

        await handler.Handle(new UpdateOrderCommand(order.Id, Items(product, 3), OrderStatus.Pending), CancellationToken.None);

        Assert.Equal(7, product.Quantity);
    }

    [Fact]
    public async Task Update_WithInsufficientStock_ReturnsBadRequestAndDoesNotSave()
    {
        var product = TestData.Product(quantity: 1);
        var order = TestData.Order(OrderStatus.Pending, (product, 2));
        SetupOrder(order);
        SetupProducts(product);
        var handler = new UpdateOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new UpdateOrderCommand(order.Id, Items(product, 4), OrderStatus.Pending), CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, result.Status);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenOrderMissing_ReturnsNotFound()
    {
        var handler = new DeleteOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(new DeleteOrderCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.Status);
        _orderRepository.Verify(r => r.Delete(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Delete_PendingOrder_ReleasesStock()
    {
        var product = TestData.Product(quantity: 7);
        var order = TestData.Order(OrderStatus.Pending, (product, 3));
        SetupOrder(order);
        SetupProducts(product);
        var handler = new DeleteOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(new DeleteOrderCommand(order.Id), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(10, product.Quantity);
        _orderRepository.Verify(r => r.Delete(order), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_DeliveredOrder_DoesNotReleaseStock()
    {
        var product = TestData.Product(quantity: 7);
        var order = TestData.Order(OrderStatus.Delivered, (product, 3));
        SetupOrder(order);
        SetupProducts(product);
        var handler = new DeleteOrderCommandHandler(_orderRepository.Object, _productRepository.Object, _unitOfWork.Object);

        await handler.Handle(new DeleteOrderCommand(order.Id), CancellationToken.None);

        Assert.Equal(7, product.Quantity);
        _orderRepository.Verify(r => r.Delete(order), Times.Once);
    }
}
