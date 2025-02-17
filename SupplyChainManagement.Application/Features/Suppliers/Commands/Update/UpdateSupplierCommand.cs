using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Update;

public record UpdateSupplierCommand(int Id, string Name, string ContactEmail, string Phone)
    : IRequest<ServiceResult<SupplierDto>>;