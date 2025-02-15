namespace SupplyChainManagement.Application.Features.Products.Dtos;

public sealed record ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int SupplierId { get; set; }
}
