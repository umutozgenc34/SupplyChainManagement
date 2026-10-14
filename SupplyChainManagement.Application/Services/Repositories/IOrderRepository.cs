using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Services.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Update(Order order);
    void Delete(Order order);
    Task<bool> AnyByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> AnyBySupplierIdAsync(int supplierId, CancellationToken cancellationToken = default);
}
