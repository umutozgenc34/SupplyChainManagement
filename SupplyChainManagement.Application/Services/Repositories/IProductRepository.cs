using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Application.Services.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetAllAsync();
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}
