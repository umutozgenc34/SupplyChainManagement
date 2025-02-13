using Core.Persistence.Repositories;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Domain.Orders;

public sealed class OrderItem : BaseEntity<Guid>
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

}
