using Microsoft.AspNetCore.Identity;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Domain.Users;

namespace SupplyChainManagement.Presentation.Security;

public static class AdminSeeder
{
    public static async Task SeedAdminAsync(this WebApplication app)
    {
        var email = app.Configuration["Seed:AdminEmail"]?.Trim().ToLowerInvariant();
        var password = app.Configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        using var scope = app.Services.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        if (await userRepository.ExistsByEmailAsync(email))
            return;

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var admin = new User
        {
            Email = email,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, password);

        await userRepository.AddAsync(admin);
        await unitOfWork.SaveChangesAsync();

        app.Logger.LogInformation("Admin user {Email} created", email);
    }
}
