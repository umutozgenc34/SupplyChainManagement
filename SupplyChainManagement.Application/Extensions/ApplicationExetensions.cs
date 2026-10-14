using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SupplyChainManagement.Application.Behaviors;
using SupplyChainManagement.Domain.Users;
using System.Reflection;

namespace SupplyChainManagement.Application.Extensions;

public static class ApplicationExetensions
{
    public static IServiceCollection AddApplicationExtension(this IServiceCollection services,Type assembly)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddValidatorsFromAssemblyContaining(assembly);
        services.AddMediatR(con => {
            con.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            con.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        return services;
    }
}
