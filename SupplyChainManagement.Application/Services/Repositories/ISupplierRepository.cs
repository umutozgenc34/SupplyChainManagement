using SupplyChainManagement.Domain.Suppliers;

namespace SupplyChainManagement.Application.Services.Repositories;

public interface ISupplierRepository
{
    Task AddAsync(Supplier supplier);
    Task<Supplier?> GetByIdAsync(int id);
    Task<List<Supplier>> GetAllAsync();
    Task UpdateAsync(Supplier supplier);
    Task DeleteAsync(int id);
}
