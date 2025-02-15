using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Queries.GetAll;

public class GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GetAllProductsQuery, ServiceResult<List<ProductDto>>>
{
    public async Task<ServiceResult<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync();
        var productAsDtos = mapper.Map<List<ProductDto>>(products);

        return ServiceResult<List<ProductDto>>.SuccessResult(productAsDtos);
    }
}