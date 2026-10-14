using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Orders.Commands.Delete;

public class DeleteOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<DeleteOrderCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        if (order is null)
            return ServiceResult.Failure("Order not found", HttpStatusCode.NotFound);

        await orderRepository.DeleteAsync(request.Id);
        return ServiceResult.SuccessResult("Order deleted successfully");
    }
}
