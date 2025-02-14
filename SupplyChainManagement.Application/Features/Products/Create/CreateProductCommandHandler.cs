using MediatR;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Products;

namespace SupplyChainManagement.Application.Features.Products.Create;

public class CreateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Quantity = request.Quantity,
            Price = request.Price,
            SupplierId = request.SupplierId
        };

        await productRepository.AddAsync(product);
        return product.Id;
    }
}