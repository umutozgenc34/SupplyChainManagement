using AutoMapper;
using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Commands.Update;

public class UpdateProductCommandHandler(IProductRepository productRepository, ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, ServiceResult<ProductDto>>
{
    public async Task<ServiceResult<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return ServiceResult<ProductDto>.Failure("Product not found", HttpStatusCode.NotFound);

        if (!await supplierRepository.ExistsAsync(request.SupplierId, cancellationToken))
            return ServiceResult<ProductDto>.Failure($"Supplier with ID {request.SupplierId} not found.");

        mapper.Map(request, product);

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedDto = mapper.Map<ProductDto>(product);

        return ServiceResult<ProductDto>.SuccessResult(updatedDto, "Product updated successfully");
    }
}
