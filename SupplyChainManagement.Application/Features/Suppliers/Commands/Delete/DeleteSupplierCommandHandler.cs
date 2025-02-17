using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Delete;

public class DeleteSupplierCommandHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<DeleteSupplierCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id);
        if (supplier is null)
            return ServiceResult.Failure("Supplier not found");

        await supplierRepository.DeleteAsync(request.Id);
        return ServiceResult.SuccessResult("Supplier deleted successfully");
    }
}