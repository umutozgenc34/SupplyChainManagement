using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Queries.GetById;

public record GetProductByIdQuery(Guid Id) : IRequest<ServiceResult<ProductDto>>;
