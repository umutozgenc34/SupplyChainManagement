using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Caching;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Delete;

[InvalidatesCache(CacheGroups.Suppliers, CacheGroups.Products)]
public record DeleteSupplierCommand(int Id) : IRequest<ServiceResult>;
