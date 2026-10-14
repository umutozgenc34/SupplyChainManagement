using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Queries.GetAll;

public class GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GetAllProductsQuery, ServiceResult<PagedResult<ProductDto>>>
{
    public async Task<ServiceResult<PagedResult<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await productRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var productAsDtos = mapper.Map<List<ProductDto>>(products);

        return ServiceResult<PagedResult<ProductDto>>.SuccessResult(
            new PagedResult<ProductDto>(productAsDtos, request.PageNumber, request.PageSize, totalCount));
    }
}
