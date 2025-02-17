using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Suppliers;
using SupplyChainManagement.Persistence.Contexts;

namespace SupplyChainManagement.Persistence.Suppliers.Concretes;

public class SupplierRepository(AppDbContext context) : ISupplierRepository
{
    public async Task AddAsync(Supplier supplier)
    {
        await context.Suppliers.AddAsync(supplier);
        await context.SaveChangesAsync();
    }

    public async Task<Supplier?> GetByIdAsync(int id) => await context.Suppliers.FindAsync(id);
    public async Task<List<Supplier>> GetAllAsync() => await context.Suppliers.ToListAsync();

    public async Task UpdateAsync(Supplier supplier)
    {
        context.Suppliers.Update(supplier);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await context.Suppliers.FindAsync(id);
        context.Suppliers.Remove(supplier);
        await context.SaveChangesAsync();
    }
}