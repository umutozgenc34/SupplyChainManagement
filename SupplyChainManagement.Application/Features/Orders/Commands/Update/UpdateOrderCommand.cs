using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Update;

public record UpdateOrderCommand(Guid Id, List<OrderItemDto> OrderItems, OrderStatus Status)
    : IRequest<ServiceResult<OrderDto>>;
