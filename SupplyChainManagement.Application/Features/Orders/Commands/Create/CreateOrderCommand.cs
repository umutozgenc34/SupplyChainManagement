using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Create;

public record CreateOrderCommand(List<OrderItemDto> OrderItems) : IRequest<ServiceResult<OrderDto>>;
