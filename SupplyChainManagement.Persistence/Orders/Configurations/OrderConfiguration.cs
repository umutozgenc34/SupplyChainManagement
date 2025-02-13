using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Persistence.Orders.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderDate).IsRequired();

        builder.Property(o => o.Status).IsRequired();

        builder.HasMany(o => o.OrderItems)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}