using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Persistence.Products.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);

        builder.Property(p => p.Quantity).IsRequired();

        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.Supplier)
              .WithMany(s => s.Products)
              .HasForeignKey(p => p.SupplierId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}