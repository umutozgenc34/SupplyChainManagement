using Core.Application.Responses;
using MediatR;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Delete;

public record DeleteSupplierCommand(int Id) : IRequest<ServiceResult>;
