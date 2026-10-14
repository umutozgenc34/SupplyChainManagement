using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupplyChainManagement.Application.Services.Caching;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Persistence.Caching;
using SupplyChainManagement.Persistence.Contexts;
using SupplyChainManagement.Persistence.Orders.Conretes;
using SupplyChainManagement.Persistence.Products.Concretes;
using SupplyChainManagement.Persistence.Suppliers.Concretes;
using SupplyChainManagement.Persistence.Users.Concretes;


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
        services.AddScoped<ISupplierRepository,SupplierRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = redisConnectionString;
                opt.InstanceName = "scm:";
            });
        }

        services.AddSingleton<ICacheService, DistributedCacheService>();
        return services;
    }
}

