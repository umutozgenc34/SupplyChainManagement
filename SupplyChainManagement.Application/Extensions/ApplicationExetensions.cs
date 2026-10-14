using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SupplyChainManagement.Application.Behaviors;
using System.Reflection;

namespace SupplyChainManagement.Application.Extensions;

public static class ApplicationExetensions
{
    public static IServiceCollection AddApplicationExtension(this IServiceCollection services,Type assembly)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssemblyContaining(assembly);
        services.AddMediatR(con => {
            con.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            con.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        return services;
    }
}
