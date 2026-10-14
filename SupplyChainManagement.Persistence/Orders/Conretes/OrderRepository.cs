using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;
using SupplyChainManagement.Persistence.Contexts;

namespace SupplyChainManagement.Persistence.Orders.Conretes;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default) =>
        await context.Orders.AddAsync(order, cancellationToken);

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(List<Order> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await context.Orders.CountAsync(cancellationToken);
        var items = await context.Orders
            .AsNoTracking()
            .Include(x => x.OrderItems)
            .OrderByDescending(x => x.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Update(Order order) => context.Orders.Update(order);

    public void Delete(Order order) => context.Orders.Remove(order);

    public async Task<bool> AnyByProductIdAsync(Guid productId, CancellationToken cancellationToken = default) =>
        await context.OrderItems.AnyAsync(x => x.ProductId == productId, cancellationToken);

    public async Task<bool> AnyBySupplierIdAsync(int supplierId, CancellationToken cancellationToken = default) =>
        await context.OrderItems.AnyAsync(x => x.Product.SupplierId == supplierId, cancellationToken);
}
