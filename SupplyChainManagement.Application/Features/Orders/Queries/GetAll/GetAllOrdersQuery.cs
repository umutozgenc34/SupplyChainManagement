using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetAll;

[Cacheable(CacheGroups.Orders)]
public record GetAllOrdersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<ServiceResult<PagedResult<OrderDto>>>;
