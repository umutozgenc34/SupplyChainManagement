using Moq;
using SupplyChainManagement.Application.Features.Products.Commands.Create;
using SupplyChainManagement.Application.Features.Products.Commands.Delete;
using SupplyChainManagement.Application.Features.Products.Commands.Update;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Products;
using System.Net;

namespace SupplyChainManagement.Application.Tests.Features.Products;

public class ProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<ISupplierRepository> _supplierRepository = new();
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private void SetupSupplierExists(int supplierId, bool exists) =>
        _supplierRepository
            .Setup(r => r.ExistsAsync(supplierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists);

    [Fact]
    public async Task Create_WithExistingSupplier_SavesProduct()
    {
        SetupSupplierExists(1, true);
        var handler = new CreateProductCommandHandler(_productRepository.Object, _supplierRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new CreateProductCommand("Bolt", 0, 2.5m, 1), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Bolt", result.Data!.Name);
        _productRepository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithMissingSupplier_ReturnsBadRequestAndDoesNotSave()
    {
        SetupSupplierExists(99, false);
        var handler = new CreateProductCommandHandler(_productRepository.Object, _supplierRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new CreateProductCommand("Bolt", 1, 2.5m, 99), CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, result.Status);
        _productRepository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenProductMissing_ReturnsNotFound()
    {
        var handler = new UpdateProductCommandHandler(_productRepository.Object, _supplierRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new UpdateProductCommand(Guid.NewGuid(), "Bolt", 1, 1m, 1), CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.Status);
    }

    [Fact]
    public async Task Update_WithMissingSupplier_ReturnsBadRequest()
    {
        var product = TestData.Product();
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        SetupSupplierExists(99, false);
        var handler = new UpdateProductCommandHandler(_productRepository.Object, _supplierRepository.Object, _unitOfWork.Object, TestData.Mapper);

        var result = await handler.Handle(new UpdateProductCommand(product.Id, "Bolt", 1, 1m, 99), CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, result.Status);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenUsedInOrders_ReturnsConflictAndDoesNotDelete()
    {
        var product = TestData.Product();
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _orderRepository.Setup(r => r.AnyByProductIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new DeleteProductCommandHandler(_productRepository.Object, _orderRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        _productRepository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenNotUsedInOrders_DeletesProduct()
    {
        var product = TestData.Product();
        _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var handler = new DeleteProductCommandHandler(_productRepository.Object, _orderRepository.Object, _unitOfWork.Object);

        var result = await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        Assert.True(result.Success);
        _productRepository.Verify(r => r.Delete(product), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
