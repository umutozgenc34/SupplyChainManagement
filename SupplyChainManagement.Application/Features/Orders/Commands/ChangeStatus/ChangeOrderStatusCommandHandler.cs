using AutoMapper;
using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Features.Orders.Rules;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Commands.ChangeStatus;

public class ChangeOrderStatusCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<ChangeOrderStatusCommand, ServiceResult<OrderDto>>
{
    public async Task<ServiceResult<OrderDto>> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            return ServiceResult<OrderDto>.Failure("Order not found", HttpStatusCode.NotFound);

        if (!order.CanTransitionTo(request.Status))
            return ServiceResult<OrderDto>.Failure($"Cannot change order status from {order.Status} to {request.Status}.", HttpStatusCode.Conflict);

        if (request.Status == OrderStatus.Canceled)
        {
            var productIds = order.OrderItems.Select(i => i.ProductId).Distinct();
            var products = (await productRepository.GetByIdsAsync(productIds, cancellationToken)).ToDictionary(p => p.Id);
            OrderStockRules.ReleaseStock(order.OrderItems, products);
        }

        order.Status = request.Status;

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var orderAsDto = mapper.Map<OrderDto>(order);

        return ServiceResult<OrderDto>.SuccessResult(orderAsDto, $"Order status changed to {order.Status}.");
    }
}
