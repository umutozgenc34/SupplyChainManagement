using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Commands.Create;

[InvalidatesCache(CacheGroups.Products)]
public record CreateProductCommand(string Name, int Quantity, decimal Price, int SupplierId) : IRequest<ServiceResult<ProductDto>>;
