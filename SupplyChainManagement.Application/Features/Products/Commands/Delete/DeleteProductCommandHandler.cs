using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Commands.Delete;

public class DeleteProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null)
            return ServiceResult.Failure("Product not found");

        await productRepository.DeleteAsync(request.Id);
        return ServiceResult.SuccessResult("Product deleted successfully");
    }
}