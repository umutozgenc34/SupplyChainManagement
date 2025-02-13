using Core.Persistence.Repositories;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Domain.Suppliers;

public sealed class Supplier : BaseEntity<int>
{
    public string Name { get; set; } = default!; 
    public string ContactEmail { get; set; } = default!;  
    public string Phone { get; set; } = default!;  

    public List<Product> Products { get; set; } = new();
}
