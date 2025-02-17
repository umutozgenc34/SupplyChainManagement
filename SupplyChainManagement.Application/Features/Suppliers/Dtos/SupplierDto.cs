namespace SupplyChainManagement.Application.Features.Suppliers.Dtos;

public sealed record SupplierDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string ContactEmail { get; init; }
    public string Phone { get; init; }
}
