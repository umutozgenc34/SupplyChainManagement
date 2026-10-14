using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Commands.Update;

[InvalidatesCache(CacheGroups.Products)]
public record UpdateProductCommand(Guid Id, string Name, int Quantity, decimal Price, int SupplierId)
    : IRequest<ServiceResult<ProductDto>>;