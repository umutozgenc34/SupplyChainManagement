using AutoMapper;
using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetById;

public class GetOrderByIdQueryHandler(IOrderRepository orderRepository,IMapper mapper) : IRequestHandler<GetOrderByIdQuery, ServiceResult<OrderDto>>
{
    public async Task<ServiceResult<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        if (order is null)
            return ServiceResult<OrderDto>.Failure("Order not found", HttpStatusCode.NotFound);

        var orderAsDto = mapper.Map<OrderDto>(order);

        return ServiceResult<OrderDto>.SuccessResult(orderAsDto);
    }
}