using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Auth.Dtos;

namespace SupplyChainManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<ServiceResult<AuthDto>>;
