using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Products;
using SupplyChainManagement.Persistence.Contexts;

namespace SupplyChainManagement.Persistence.Products.Concretes;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id) => await context.Products.FindAsync(id);
    public async Task<List<Product>> GetAllAsync() => await context.Products.ToListAsync();
    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await context.Products.FindAsync(id);
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }
}