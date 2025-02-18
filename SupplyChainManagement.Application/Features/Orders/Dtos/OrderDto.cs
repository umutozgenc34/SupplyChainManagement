using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Dtos;

public record OrderDto
{
    public Guid Id { get; init; }
    public DateTime OrderDate { get; init; }
    public OrderStatus Status { get; init; }
    public List<OrderItemDto> OrderItems { get; init; }
    public decimal TotalPrice { get; init; }
}
