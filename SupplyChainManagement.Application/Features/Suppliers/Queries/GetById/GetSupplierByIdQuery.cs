using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetById;

public record GetSupplierByIdQuery(int Id) : IRequest<ServiceResult<SupplierDto>>;