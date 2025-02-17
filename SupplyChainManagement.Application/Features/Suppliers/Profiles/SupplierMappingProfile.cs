using AutoMapper;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Create;
using SupplyChainManagement.Application.Features.Suppliers.Commands.Update;
using SupplyChainManagement.Application.Features.Suppliers.Dtos;
using SupplyChainManagement.Domain.Suppliers;

namespace SupplyChainManagement.Application.Features.Suppliers.Profiles;

public class SupplierMappingProfile : Profile
{
    public SupplierMappingProfile()
    {
        CreateMap<CreateSupplierCommand, Supplier>();
        CreateMap<SupplierDto, Supplier>().ReverseMap();
        CreateMap<UpdateSupplierCommand, Supplier>();
    }
}
