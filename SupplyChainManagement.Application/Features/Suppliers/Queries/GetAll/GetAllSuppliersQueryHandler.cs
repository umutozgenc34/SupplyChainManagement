using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetAll;

public class GetAllSuppliersQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<GetAllSuppliersQuery, ServiceResult<List<SupplierDto>>>
{
    public async Task<ServiceResult<List<SupplierDto>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await supplierRepository.GetAllAsync();
        var supplierAsDtos = mapper.Map<List<SupplierDto>>(suppliers);
        return ServiceResult<List<SupplierDto>>.SuccessResult(supplierAsDtos);
    }
}