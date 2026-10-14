using SupplyChainManagement.Application.Services.Repositories;

namespace SupplyChainManagement.Persistence.Contexts;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
