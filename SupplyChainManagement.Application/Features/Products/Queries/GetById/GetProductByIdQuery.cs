using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Queries.GetById;

[Cacheable(CacheGroups.Products)]
public record GetProductByIdQuery(Guid Id) : IRequest<ServiceResult<ProductDto>>;
