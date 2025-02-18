using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetAll;

public class GetAllOrdersQueryHandler(IOrderRepository orderRepository,IMapper mapper) : IRequestHandler<GetAllOrdersQuery, ServiceResult<List<OrderDto>>>
{
    public async Task<ServiceResult<List<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetAllAsync();
        var orderAsDtos = mapper.Map<List<OrderDto>>(orders);

        return ServiceResult<List<OrderDto>>.SuccessResult(orderAsDtos);
    }
}