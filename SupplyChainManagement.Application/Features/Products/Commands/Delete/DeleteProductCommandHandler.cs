using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Commands.Delete;

public class DeleteProductCommandHandler(IProductRepository productRepository, IOrderRepository orderRepository)
    : IRequestHandler<DeleteProductCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id);
        if (product is null)
            return ServiceResult.Failure("Product not found", HttpStatusCode.NotFound);

        if (await orderRepository.AnyByProductIdAsync(request.Id))
            return ServiceResult.Failure("Product is used in orders and cannot be deleted.", HttpStatusCode.Conflict);

        await productRepository.DeleteAsync(request.Id);
        return ServiceResult.SuccessResult("Product deleted successfully");
    }
}