using Core.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;
using SupplyChainManagement.Application.Features.Auth.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Application.Services.Security;
using SupplyChainManagement.Domain.Users;

namespace SupplyChainManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ITokenService tokenService, IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, ServiceResult<AuthDto>>
{
    public async Task<ServiceResult<AuthDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            return ServiceResult<AuthDto>.Failure("Email is already registered.", HttpStatusCode.Conflict);

        var user = new User
        {
            Email = email,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<AuthDto>.SuccessResult(tokenService.CreateToken(user), "User registered successfully.", HttpStatusCode.Created);
    }
}
