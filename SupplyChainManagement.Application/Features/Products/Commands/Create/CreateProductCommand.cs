using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Commands.Create;

public record CreateProductCommand(string Name, int Quantity, decimal Price, int SupplierId) : IRequest<ServiceResult<ProductDto>>;
