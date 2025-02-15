using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Application.Features.Products.Commands.Create;

public class CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper) : IRequestHandler<CreateProductCommand, ServiceResult<ProductDto>>
{
    public async Task<ServiceResult<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = mapper.Map<Product>(request);

        await productRepository.AddAsync(product);

        var productAsDto = mapper.Map<ProductDto>(product);

        return ServiceResult<ProductDto>.SuccessResult(productAsDto, "Product created successfully");

    }
}