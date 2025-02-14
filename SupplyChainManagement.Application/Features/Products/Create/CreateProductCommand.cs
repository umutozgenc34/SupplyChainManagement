using MediatR;

namespace SupplyChainManagement.Application.Features.Products.Create;

public record CreateProductCommand(string Name, int Quantity, decimal Price, int SupplierId) : IRequest<Guid>;
