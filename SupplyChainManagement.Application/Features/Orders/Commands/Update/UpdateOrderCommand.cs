using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Update;

public record UpdateOrderCommand(Guid Id, List<OrderItemDto> OrderItems, int Status)
    : IRequest<ServiceResult<OrderDto>>;