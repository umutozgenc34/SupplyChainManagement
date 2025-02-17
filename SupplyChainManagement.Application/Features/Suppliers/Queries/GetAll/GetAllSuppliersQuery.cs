using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetAll;


public record GetAllSuppliersQuery() : IRequest<ServiceResult<List<SupplierDto>>>;