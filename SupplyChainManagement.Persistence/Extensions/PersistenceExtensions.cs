using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Persistence.Contexts;
using SupplyChainManagement.Persistence.Products.Concretes;


namespace SupplyChainManagement.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceExtensions(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}

