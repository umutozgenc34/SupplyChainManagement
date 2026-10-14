using Core.Application.Responses;
using MediatR;
using SupplyChainManagement.Application.Features.Auth.Dtos;

namespace SupplyChainManagement.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password) : IRequest<ServiceResult<AuthDto>>;
