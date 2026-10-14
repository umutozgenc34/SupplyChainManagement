using AutoMapper;
using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Features.Orders.Rules;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Update;

public class UpdateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateOrderCommand, ServiceResult<OrderDto>>
{
    public async Task<ServiceResult<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            return ServiceResult<OrderDto>.Failure("Order not found", HttpStatusCode.NotFound);

        var productIds = order.OrderItems.Select(i => i.ProductId)
            .Concat(request.OrderItems.Select(i => i.ProductId))
            .Distinct();
        var products = (await productRepository.GetByIdsAsync(productIds, cancellationToken)).ToDictionary(p => p.Id);

        if (OrderStockRules.HoldsStock(order.Status))
            OrderStockRules.ReleaseStock(order.OrderItems, products);

        var orderItemsResult = OrderStockRules.CreateOrderItems(request.OrderItems, products, OrderStockRules.HoldsStock(request.Status));
        if (!orderItemsResult.Success)
            return ServiceResult<OrderDto>.Failure(orderItemsResult.Message);

        order.Status = request.Status;
        order.OrderItems.Clear();
        order.OrderItems.AddRange(orderItemsResult.Data!);

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var orderAsDto = mapper.Map<OrderDto>(order);

        return ServiceResult<OrderDto>.SuccessResult(orderAsDto, "Order updated successfully");
    }
}
