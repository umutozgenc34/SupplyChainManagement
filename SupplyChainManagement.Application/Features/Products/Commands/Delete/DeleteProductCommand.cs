using Core.Application.Responses;
using MediatR;

namespace SupplyChainManagement.Application.Features.Products.Commands.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<ServiceResult>;