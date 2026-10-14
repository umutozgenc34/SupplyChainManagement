using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetById;

[Cacheable(CacheGroups.Orders)]
public record GetOrderByIdQuery(Guid Id) : IRequest<ServiceResult<OrderDto>>;