using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetById;

[Cacheable(CacheGroups.Suppliers)]
public record GetSupplierByIdQuery(int Id) : IRequest<ServiceResult<SupplierDto>>;