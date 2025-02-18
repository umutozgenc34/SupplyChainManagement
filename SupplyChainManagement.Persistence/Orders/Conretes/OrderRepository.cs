using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;
using SupplyChainManagement.Persistence.Contexts;

namespace SupplyChainManagement.Persistence.Orders.Conretes;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task AddAsync(Order order)
    {
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id) =>
        await context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Order>> GetAllAsync() =>
        await context.Orders.Include(x => x.OrderItems).ToListAsync();

    public async Task UpdateAsync(Order order)
    {
        context.Orders.Update(order);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await context.Orders.FindAsync(id);
        context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }
}
