using SupplyChainManagement.Domain.Suppliers;

namespace SupplyChainManagement.Application.Services.Repositories;

public interface ISupplierRepository
{
    Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    void Update(Supplier supplier);
    void Delete(Supplier supplier);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
