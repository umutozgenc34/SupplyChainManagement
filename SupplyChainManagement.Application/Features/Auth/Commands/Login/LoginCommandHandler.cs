using Core.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Net;
using SupplyChainManagement.Application.Features.Auth.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Application.Services.Security;
using SupplyChainManagement.Domain.Users;

namespace SupplyChainManagement.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ITokenService tokenService, IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, ServiceResult<AuthDto>>
{
    public async Task<ServiceResult<AuthDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null)
            return ServiceResult<AuthDto>.Failure("Invalid email or password.", HttpStatusCode.Unauthorized);

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
            return ServiceResult<AuthDto>.Failure("Invalid email or password.", HttpStatusCode.Unauthorized);

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return ServiceResult<AuthDto>.SuccessResult(tokenService.CreateToken(user), "Login successful.");
    }
}
