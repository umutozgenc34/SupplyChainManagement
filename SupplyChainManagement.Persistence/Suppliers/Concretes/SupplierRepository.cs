using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Suppliers;
using SupplyChainManagement.Persistence.Contexts;

namespace SupplyChainManagement.Persistence.Suppliers.Concretes;

public class SupplierRepository(AppDbContext context) : ISupplierRepository
{
    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default) =>
        await context.Suppliers.AddAsync(supplier, cancellationToken);

    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Suppliers.FindAsync([id], cancellationToken);

    public async Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await context.Suppliers.CountAsync(cancellationToken);
        var items = await context.Suppliers
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Update(Supplier supplier) => context.Suppliers.Update(supplier);

    public void Delete(Supplier supplier) => context.Suppliers.Remove(supplier);

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Suppliers.AnyAsync(x => x.Id == id, cancellationToken);
}
