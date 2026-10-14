using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Orders.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Orders.Queries.GetAll;

public class GetAllOrdersQueryHandler(IOrderRepository orderRepository,IMapper mapper) : IRequestHandler<GetAllOrdersQuery, ServiceResult<PagedResult<OrderDto>>>
{
    public async Task<ServiceResult<PagedResult<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await orderRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var orderAsDtos = mapper.Map<List<OrderDto>>(orders);

        return ServiceResult<PagedResult<OrderDto>>.SuccessResult(
            new PagedResult<OrderDto>(orderAsDtos, request.PageNumber, request.PageSize, totalCount));
    }
}
