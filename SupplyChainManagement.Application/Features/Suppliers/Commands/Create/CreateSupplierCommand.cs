using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Create;

[InvalidatesCache(CacheGroups.Suppliers)]
public record CreateSupplierCommand(string Name, string ContactEmail, string Phone)
    : IRequest<ServiceResult<SupplierDto>>;