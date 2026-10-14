using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Update;

[InvalidatesCache(CacheGroups.Orders, CacheGroups.Products)]
public record UpdateOrderCommand(Guid Id, List<OrderItemDto> OrderItems)
    : IRequest<ServiceResult<OrderDto>>;
