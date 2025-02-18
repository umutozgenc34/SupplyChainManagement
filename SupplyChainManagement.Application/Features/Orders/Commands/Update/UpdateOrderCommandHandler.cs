using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Update;

public class UpdateOrderCommandHandler(IOrderRepository orderRepository,IMapper mapper) : IRequestHandler<UpdateOrderCommand, ServiceResult<OrderDto>>
{
    public async Task<ServiceResult<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        if (order is null)
        {
            return ServiceResult<OrderDto>.Failure("Order not found");
        }

        mapper.Map(request, order);

        await orderRepository.UpdateAsync(order);

        var orderAsDto = mapper.Map<OrderDto>(order); 

        return ServiceResult<OrderDto>.SuccessResult(orderAsDto, "Order updated successfully");
    }
}