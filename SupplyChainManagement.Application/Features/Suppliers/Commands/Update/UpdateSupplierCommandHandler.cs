using AutoMapper;
using Core.Application.Responses;
using MediatR;
using System.Net;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<UpdateSupplierCommand, ServiceResult<SupplierDto>>
{
    public async Task<ServiceResult<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return ServiceResult<SupplierDto>.Failure("Supplier not found", HttpStatusCode.NotFound);

        mapper.Map(request, supplier);
        supplierRepository.Update(supplier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var supplierAsDto = mapper.Map<SupplierDto>(supplier);
        return ServiceResult<SupplierDto>.SuccessResult(supplierAsDto, "Supplier updated successfully");
    }
}
