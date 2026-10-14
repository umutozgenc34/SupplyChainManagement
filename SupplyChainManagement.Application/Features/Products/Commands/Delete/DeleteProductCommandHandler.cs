using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Products.Commands.Delete;

public class DeleteProductCommandHandler(IProductRepository productRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return ServiceResult.Failure("Product not found", HttpStatusCode.NotFound);

        if (await orderRepository.AnyByProductIdAsync(request.Id, cancellationToken))
            return ServiceResult.Failure("Product is used in orders and cannot be deleted.", HttpStatusCode.Conflict);

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.SuccessResult("Product deleted successfully");
    }
}
