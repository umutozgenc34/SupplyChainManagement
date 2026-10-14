using Microsoft.EntityFrameworkCore;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Users;
using SupplyChainManagement.Persistence.Contexts;

namespace SupplyChainManagement.Persistence.Users.Concretes;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await context.Users.AddAsync(user, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await context.Users.AnyAsync(x => x.Email == email, cancellationToken);
}
