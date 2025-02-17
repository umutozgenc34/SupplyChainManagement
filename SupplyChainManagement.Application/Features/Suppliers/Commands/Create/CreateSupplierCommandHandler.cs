using AutoMapper;
using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Suppliers;

namespace SupplyChainManagement.Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
    : IRequestHandler<CreateSupplierCommand, ServiceResult<SupplierDto>>
{
    public async Task<ServiceResult<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = mapper.Map<Supplier>(request);
        await supplierRepository.AddAsync(supplier);
        var supplierAsDto = mapper.Map<SupplierDto>(supplier);

        return ServiceResult<SupplierDto>.SuccessResult(supplierAsDto, "Supplier created successfully");
    }
}