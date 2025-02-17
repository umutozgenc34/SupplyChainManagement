using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Create;

public record CreateSupplierCommand(string Name, string ContactEmail, string Phone)
    : IRequest<ServiceResult<SupplierDto>>;