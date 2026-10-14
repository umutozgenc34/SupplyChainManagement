using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Domain.Orders;
using SupplyChainManagement.Domain.Products;
using SupplyChainManagement.Domain.Suppliers;
using SupplyChainManagement.Domain.Users;
using System.Reflection;

namespace SupplyChainManagement.Persistence.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; } 
    public DbSet<Product> Products { get; set; } 
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
