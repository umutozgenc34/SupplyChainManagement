using Core.Persistence.Repositories;

namespace SupplyChainManagement.Domain.Orders;

public sealed class Order : BaseEntity<Guid>
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        [OrderStatus.Pending] = [OrderStatus.Approved, OrderStatus.Canceled],
        [OrderStatus.Approved] = [OrderStatus.Shipped, OrderStatus.Canceled],
        [OrderStatus.Shipped] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [],
        [OrderStatus.Canceled] = []
    };

    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public List<OrderItem> OrderItems { get; set; } = new();

    public decimal TotalPrice => OrderItems.Sum(item => item.Price * item.Quantity);

    public bool IsEditable => Status == OrderStatus.Pending;

    public bool IsDeletable => Status is OrderStatus.Pending or OrderStatus.Canceled;

    public bool CanTransitionTo(OrderStatus status) =>
        AllowedTransitions.TryGetValue(Status, out var allowed) && allowed.Contains(status);
}
