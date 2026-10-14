using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Delete;

public class DeleteSupplierCommandHandler(ISupplierRepository supplierRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSupplierCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return ServiceResult.Failure("Supplier not found", HttpStatusCode.NotFound);

        if (await orderRepository.AnyBySupplierIdAsync(request.Id, cancellationToken))
            return ServiceResult.Failure("Supplier has products used in orders and cannot be deleted.", HttpStatusCode.Conflict);

        supplierRepository.Delete(supplier);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.SuccessResult("Supplier deleted successfully");
    }
}
