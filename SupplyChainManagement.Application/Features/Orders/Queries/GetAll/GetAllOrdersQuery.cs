using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetAll;

public record GetAllOrdersQuery() : IRequest<ServiceResult<List<OrderDto>>>;