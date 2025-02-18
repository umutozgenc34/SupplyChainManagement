using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetById;

public record GetOrderByIdQuery(Guid Id) : IRequest<ServiceResult<OrderDto>>;