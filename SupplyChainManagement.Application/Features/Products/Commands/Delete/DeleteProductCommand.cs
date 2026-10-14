using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;

namespace SupplyChainManagement.Application.Features.Products.Commands.Delete;

[InvalidatesCache(CacheGroups.Products)]
public record DeleteProductCommand(Guid Id) : IRequest<ServiceResult>;