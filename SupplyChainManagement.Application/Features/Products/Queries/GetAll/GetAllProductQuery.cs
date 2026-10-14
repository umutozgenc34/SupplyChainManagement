using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Queries.GetAll;

[Cacheable(CacheGroups.Products)]
public record GetAllProductsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<ServiceResult<PagedResult<ProductDto>>>;
