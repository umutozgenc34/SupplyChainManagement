using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Features.Orders.Rules;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Orders;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Create;

public class CreateOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateOrderCommand, ServiceResult<OrderDto>>
{
    public async Task<ServiceResult<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var productIds = request.OrderItems.Select(i => i.ProductId).Distinct();
        var products = (await productRepository.GetByIdsAsync(productIds, cancellationToken)).ToDictionary(p => p.Id);

        var orderItemsResult = OrderStockRules.ReserveStock(request.OrderItems, products);
        if (!orderItemsResult.Success)
            return ServiceResult<OrderDto>.Failure(orderItemsResult.Message);

        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            OrderItems = orderItemsResult.Data!
        };

        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var responseAsDto = mapper.Map<OrderDto>(order);

        return ServiceResult<OrderDto>.SuccessResult(responseAsDto, "Order created successfully.");
    }
}
