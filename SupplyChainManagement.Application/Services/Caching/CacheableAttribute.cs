namespace SupplyChainManagement.Application.Services.Caching;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CacheableAttribute(string group, int expirationSeconds = 300) : Attribute
{
    public string Group { get; } = group;
    public TimeSpan Expiration { get; } = TimeSpan.FromSeconds(expirationSeconds);
}
