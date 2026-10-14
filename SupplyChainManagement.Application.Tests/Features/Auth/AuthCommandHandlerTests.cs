using Microsoft.AspNetCore.Identity;
using Moq;
using SupplyChainManagement.Application.Features.Auth.Commands.Login;
using SupplyChainManagement.Application.Features.Auth.Commands.Register;
using SupplyChainManagement.Application.Features.Auth.Dtos;
using SupplyChainManagement.Application.Services.Repositories;
using SupplyChainManagement.Application.Services.Security;
using SupplyChainManagement.Domain.Users;
using System.Net;

namespace SupplyChainManagement.Application.Tests.Features.Auth;

public class AuthCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthCommandHandlerTests()
    {
        _tokenService
            .Setup(t => t.CreateToken(It.IsAny<User>()))
            .Returns((User u) => new AuthDto("token", DateTime.UtcNow.AddHours(1), u.Email, u.Role));
    }

    private User ExistingUser(string email, string password)
    {
        var user = new User { Id = Guid.NewGuid(), Email = email, Role = UserRole.User };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        _userRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        return user;
    }

    [Fact]
    public async Task Register_WithNewEmail_SavesHashedUserAndReturnsToken()
    {
        User? savedUser = null;
        _userRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback((User u, CancellationToken _) => savedUser = u);
        var handler = new RegisterCommandHandler(_userRepository.Object, _passwordHasher, _tokenService.Object, _unitOfWork.Object);

        var result = await handler.Handle(new RegisterCommand("  New@Example.com ", "Secret123"), CancellationToken.None);

        Assert.Equal(HttpStatusCode.Created, result.Status);
        Assert.Equal("token", result.Data!.AccessToken);
        Assert.NotNull(savedUser);
        Assert.Equal("new@example.com", savedUser!.Email);
        Assert.Equal(UserRole.User, savedUser.Role);
        Assert.NotEqual("Secret123", savedUser.PasswordHash);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsConflict()
    {
        _userRepository.Setup(r => r.ExistsByEmailAsync("taken@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new RegisterCommandHandler(_userRepository.Object, _passwordHasher, _tokenService.Object, _unitOfWork.Object);

        var result = await handler.Handle(new RegisterCommand("taken@example.com", "Secret123"), CancellationToken.None);

        Assert.Equal(HttpStatusCode.Conflict, result.Status);
        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Login_WithCorrectPassword_ReturnsToken()
    {
        ExistingUser("user@example.com", "Secret123");
        var handler = new LoginCommandHandler(_userRepository.Object, _passwordHasher, _tokenService.Object, _unitOfWork.Object);

        var result = await handler.Handle(new LoginCommand("USER@example.com", "Secret123"), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("user@example.com", result.Data!.Email);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        ExistingUser("user@example.com", "Secret123");
        var handler = new LoginCommandHandler(_userRepository.Object, _passwordHasher, _tokenService.Object, _unitOfWork.Object);

        var result = await handler.Handle(new LoginCommand("user@example.com", "Wrong123"), CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, result.Status);
        _tokenService.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsSameUnauthorizedMessage()
    {
        var handler = new LoginCommandHandler(_userRepository.Object, _passwordHasher, _tokenService.Object, _unitOfWork.Object);

        var result = await handler.Handle(new LoginCommand("nobody@example.com", "Secret123"), CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, result.Status);
        Assert.Equal("Invalid email or password.", result.Message);
    }

    [Theory]
    [InlineData("Secret123", true)]
    [InlineData("short1A", false)]
    [InlineData("nouppercase1", false)]
    [InlineData("NoDigitsHere", false)]
    public void RegisterValidator_EnforcesPasswordRules(string password, bool expectedValid)
    {
        var result = new RegisterCommandValidator().Validate(new RegisterCommand("user@example.com", password));

        Assert.Equal(expectedValid, result.IsValid);
    }
}
