using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Domain.Suppliers;

namespace SupplyChainManagement.Persistence.Suppliers.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);

        builder.Property(s => s.ContactEmail).IsRequired().HasMaxLength(255);

        builder.Property(s => s.Phone).IsRequired().HasMaxLength(11);

        builder.HasMany(s => s.Products)
               .WithOne(p => p.Supplier)
               .HasForeignKey(p => p.SupplierId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}