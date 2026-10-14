using SupplyChainManagement.Domain.Users;

namespace SupplyChainManagement.Application.Features.Auth.Dtos;

public record AuthDto(string AccessToken, DateTime ExpiresAt, string Email, UserRole Role);
