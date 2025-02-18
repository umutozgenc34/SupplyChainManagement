using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Create;

public class CreateOrderCommandHandler(IOrderRepository orderRepository,IMapper mapper,IProductRepository productRepository) : IRequestHandler<CreateOrderCommand, ServiceResult<OrderDto>>
{
    public async Task<ServiceResult<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in request.OrderItems)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product is null)
            {
                return ServiceResult<OrderDto>.Failure($"Product with ID {item.ProductId} not found.");
            }

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price 
            };
            orderItems.Add(orderItem);
        }

        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            OrderItems = orderItems
        };

        await orderRepository.AddAsync(order);

        var responseAsDto = mapper.Map<OrderDto>(order);

        return ServiceResult<OrderDto>.SuccessResult(responseAsDto, "Order created successfully.");
    }
}