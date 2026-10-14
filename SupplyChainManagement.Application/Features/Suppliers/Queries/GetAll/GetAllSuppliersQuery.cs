using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetAll;

[Cacheable(CacheGroups.Suppliers)]
public record GetAllSuppliersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<ServiceResult<PagedResult<SupplierDto>>>;
