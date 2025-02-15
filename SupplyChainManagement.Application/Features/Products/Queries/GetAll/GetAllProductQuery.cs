using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;

namespace SupplyChainManagement.Application.Features.Products.Queries.GetAll;

public record GetAllProductsQuery() : IRequest<ServiceResult<List<ProductDto>>>;