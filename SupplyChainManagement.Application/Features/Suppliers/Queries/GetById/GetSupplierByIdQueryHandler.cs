using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetById;

public class GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<GetSupplierByIdQuery, ServiceResult<SupplierDto>>
{
    public async Task<ServiceResult<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id);
        if (supplier is null)
            return ServiceResult<SupplierDto>.Failure("Supplier not found");

        var supplierAsDto = mapper.Map<SupplierDto>(supplier);
        return ServiceResult<SupplierDto>.SuccessResult(supplierAsDto);
    }
}