using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Commands.Update;

public class UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, ServiceResult<ProductDto>>
{
    public async Task<ServiceResult<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null)
            return ServiceResult<ProductDto>.Failure("Product not found");

        mapper.Map(request, product); 

        await productRepository.UpdateAsync(product);
        var updatedDto = mapper.Map<ProductDto>(product); 

        return ServiceResult<ProductDto>.SuccessResult(updatedDto, "Product updated successfully");
    }
}