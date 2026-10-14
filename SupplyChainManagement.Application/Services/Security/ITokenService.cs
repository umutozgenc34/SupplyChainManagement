using SupplyChainManagement.Application.Features.Auth.Dtos;
using SupplyChainManagement.Domain.Users;

namespace SupplyChainManagement.Application.Services.Security;

public interface ITokenService
{
    AuthDto CreateToken(User user);
}
