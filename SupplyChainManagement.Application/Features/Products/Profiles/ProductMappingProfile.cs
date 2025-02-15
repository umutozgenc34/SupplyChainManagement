using AutoMapper;
using SupplyChainManagement.Application.Features.Products.Create;
using SupplyChainManagement.Application.Features.Products.Dtos;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Application.Features.Products.Profiles;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<Product, ProductDto>().ReverseMap(); ;

    }
}
