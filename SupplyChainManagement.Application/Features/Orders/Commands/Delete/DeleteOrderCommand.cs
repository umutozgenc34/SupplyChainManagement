using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Delete;

[InvalidatesCache(CacheGroups.Orders, CacheGroups.Products)]
public record DeleteOrderCommand(Guid Id) : IRequest<ServiceResult>;