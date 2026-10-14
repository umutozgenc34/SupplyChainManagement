using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Update;

public class UpdateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper) : IRequestHandler<UpdateOrderCommand, ServiceResult<OrderDto>>
{
    public async Task<ServiceResult<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        if (order is null)
        {
            return ServiceResult<OrderDto>.Failure("Order not found");
        }

        var orderItems = new List<OrderItem>();

        foreach (var item in request.OrderItems)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product is null)
            {
                return ServiceResult<OrderDto>.Failure($"Product with ID {item.ProductId} not found.");
            }

            orderItems.Add(new OrderItem(item.ProductId, item.Quantity, product.Price));
        }

        order.Status = request.Status;
        order.OrderItems.Clear();
        order.OrderItems.AddRange(orderItems);

        await orderRepository.UpdateAsync(order);

        var orderAsDto = mapper.Map<OrderDto>(order);

        return ServiceResult<OrderDto>.SuccessResult(orderAsDto, "Order updated successfully");
    }
}
