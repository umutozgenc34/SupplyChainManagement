using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Commands.ChangeStatus;

public record ChangeOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<ServiceResult<OrderDto>>;
