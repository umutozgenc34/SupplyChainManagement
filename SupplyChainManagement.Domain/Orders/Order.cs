using Core.Persistence.Repositories;

namespace SupplyChainManagement.Domain.Orders;

public sealed class Order : BaseEntity<Guid>
{
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;  

    public List<OrderItem> OrderItems { get; set; } = new();
}
