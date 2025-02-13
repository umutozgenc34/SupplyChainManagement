using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupplyChainManagement.Persistence.Contexts;


namespace SupplyChainManagement.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceExtensions(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });

        return services;
    }
}

