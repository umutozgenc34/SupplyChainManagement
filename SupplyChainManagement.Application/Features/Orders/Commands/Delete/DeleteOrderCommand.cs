using Core.Application.Responses;
using MediatR;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Delete;

public record DeleteOrderCommand(Guid Id) : IRequest<ServiceResult>;