using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SupplyChainManagement.Application.Extensions;

public static class ApplicationExetensions
{
    public static IServiceCollection AddApplicationExtension(this IServiceCollection services)
    {
        services.AddMediatR(con => {
            con.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        return services;
    }
}
