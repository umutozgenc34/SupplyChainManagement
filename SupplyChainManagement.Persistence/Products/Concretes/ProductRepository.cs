using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Products;
using SupplyChainManagement.Persistence.Contexts;

namespace SupplyChainManagement.Persistence.Products.Concretes;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await context.Products.AddAsync(product, cancellationToken);

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Products.FindAsync([id], cancellationToken);

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) =>
        await context.Products.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await context.Products.CountAsync(cancellationToken);
        var items = await context.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Update(Product product) => context.Products.Update(product);

    public void Delete(Product product) => context.Products.Remove(product);
}
