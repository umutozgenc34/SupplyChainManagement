using Core.Persistence.Repositories;

namespace SupplyChainManagement.Domain.Users;

public sealed class User : BaseEntity<Guid>
{
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; }
}
