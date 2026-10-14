using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetAll;

public record GetAllOrdersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<ServiceResult<PagedResult<OrderDto>>>;
