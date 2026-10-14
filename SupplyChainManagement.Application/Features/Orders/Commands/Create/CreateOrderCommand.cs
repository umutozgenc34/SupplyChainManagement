using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Orders.Dtos;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Create;

[InvalidatesCache(CacheGroups.Orders, CacheGroups.Products)]
public record CreateOrderCommand(List<OrderItemDto> OrderItems) : IRequest<ServiceResult<OrderDto>>;
