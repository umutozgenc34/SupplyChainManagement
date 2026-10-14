namespace SupplyChainManagement.Application.Services.Caching;

[AttributeUsage(AttributeTargets.Class)]
public sealed class InvalidatesCacheAttribute(params string[] groups) : Attribute
{
    public string[] Groups { get; } = groups;
}
