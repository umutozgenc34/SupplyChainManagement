using Moq;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Delete;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Suppliers;
using System.Net;

namespace SupplyChainManagement.Application.Tests.Features.Suppliers;

public class SupplierCommandHandlerTests
{
    private readonly Mock<ISupplierRepository> _supplierRepository = new();
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private DeleteSupplierCommandHandler CreateHandler() =>
        new(_supplierRepository.Object, _orderRepository.Object, _unitOfWork.Object);

    [Fact]
    public async Task Delete_WhenSupplierMissing_ReturnsNotFound()
    {
        var result = await CreateHandler().Handle(new DeleteSupplierCommand(1), CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.Status);
    }

    [Fact]
    public async Task Delete_WhenProductsUsedInOrders_ReturnsConflictAndDoesNotDelete()
    {
        var supplier = new Supplier { Id = 1, Name = "Acme", ContactEmail = "a@acme.com", Phone = "05550000000" };
        _supplierRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(supplier);
        _orderRepository.Setup(r => r.AnyBySupplierIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateHandler().Handle(new DeleteSupplierCommand(1), CancellationToken.None);

        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        _supplierRepository.Verify(r => r.Delete(It.IsAny<Supplier>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenNotUsedInOrders_DeletesSupplier()
    {
        var supplier = new Supplier { Id = 1, Name = "Acme", ContactEmail = "a@acme.com", Phone = "05550000000" };
        _supplierRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(supplier);

        var result = await CreateHandler().Handle(new DeleteSupplierCommand(1), CancellationToken.None);

        Assert.True(result.Success);
        _supplierRepository.Verify(r => r.Delete(supplier), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
