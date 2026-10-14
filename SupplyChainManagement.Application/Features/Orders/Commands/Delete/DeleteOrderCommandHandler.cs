using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Features.Orders.Rules;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Delete;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteOrderCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            return ServiceResult.Failure("Order not found", HttpStatusCode.NotFound);

        if (!order.IsDeletable)
            return ServiceResult.Failure($"Only pending or canceled orders can be deleted. Current status: {order.Status}.", HttpStatusCode.Conflict);

        if (order.Status == OrderStatus.Pending)
        {
            var productIds = order.OrderItems.Select(i => i.ProductId).Distinct();
            var products = (await productRepository.GetByIdsAsync(productIds, cancellationToken)).ToDictionary(p => p.Id);
            OrderStockRules.ReleaseStock(order.OrderItems, products);
        }

        orderRepository.Delete(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.SuccessResult("Order deleted successfully");
    }
}
