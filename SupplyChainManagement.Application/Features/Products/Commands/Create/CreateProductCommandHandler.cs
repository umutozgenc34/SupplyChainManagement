using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Application.Features.Products.Commands.Create;

public class CreateProductCommandHandler(IProductRepository productRepository, ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateProductCommand, ServiceResult<ProductDto>>
{
    public async Task<ServiceResult<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!await supplierRepository.ExistsAsync(request.SupplierId, cancellationToken))
            return ServiceResult<ProductDto>.Failure($"Supplier with ID {request.SupplierId} not found.");

        var product = mapper.Map<Product>(request);

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var productAsDto = mapper.Map<ProductDto>(product);

        return ServiceResult<ProductDto>.SuccessResult(productAsDto, "Product created successfully");
    }
}
