using System.Text.Json.Serialization;

namespace SupplyChainManagement.Application.Features.Orders.Dtos;

public record OrderItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}
