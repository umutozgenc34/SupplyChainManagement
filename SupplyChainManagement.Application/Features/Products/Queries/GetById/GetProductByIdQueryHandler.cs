using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Queries.GetById;

public class GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<GetProductByIdQuery, ServiceResult<ProductDto>>
{
    public async Task<ServiceResult<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null)
            return ServiceResult<ProductDto>.Failure("Product not found");

        var productAsDto = mapper.Map<ProductDto>(product);
        return ServiceResult<ProductDto>.SuccessResult(productAsDto);
    }
}