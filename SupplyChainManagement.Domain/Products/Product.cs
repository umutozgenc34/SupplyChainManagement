using Core.Persistence.Repositories;
using SupplyChainManagement.Domain.Suppliers;

namespace SupplyChainManagement.Domain.Products;

public sealed class Product : BaseEntity<Guid>
{
    public string Name { get; set; } = default!;  
    public int Quantity { get; set; } 
    public decimal Price { get; set; }  

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
}
