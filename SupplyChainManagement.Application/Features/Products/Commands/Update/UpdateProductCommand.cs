using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Commands.Update;

public record UpdateProductCommand(Guid Id, string Name, int Quantity, decimal Price, int SupplierId)
    : IRequest<ServiceResult<ProductDto>>;