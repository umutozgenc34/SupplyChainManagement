using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;
using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Application.Features.Suppliers.Queries.GetAll;

public class GetAllSuppliersQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<GetAllSuppliersQuery, ServiceResult<PagedResult<SupplierDto>>>
{
    public async Task<ServiceResult<PagedResult<SupplierDto>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var (suppliers, totalCount) = await supplierRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var supplierAsDtos = mapper.Map<List<SupplierDto>>(suppliers);

        return ServiceResult<PagedResult<SupplierDto>>.SuccessResult(
            new PagedResult<SupplierDto>(supplierAsDtos, request.PageNumber, request.PageSize, totalCount));
    }
}
