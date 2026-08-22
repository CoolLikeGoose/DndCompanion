using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Auth.Login;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Auth;

public class LoginUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly LoginUserService _service;

    public LoginUserServiceTests()
    {
        _service = new LoginUserService(_userRepository.Object, _passwordHasher.Object);
    }

    [Fact]
    public async Task Fails_WhenEmailEmpty()
    {
        var result = await _service.ExecuteAsync(new LoginUserCommand("", "password"));
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenPasswordEmpty()
    {
        var result = await _service.ExecuteAsync(new LoginUserCommand("test@example.com", ""));
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenUserNotFound()
    {
        _userRepository
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _service.ExecuteAsync(new LoginUserCommand("test@example.com", "password"));

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Fails_WhenPasswordInvalid()
    {
        var user = User.Create("TestUser", "test@example.com", "hashed-password");

        _userRepository
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var result = await _service.ExecuteAsync(new LoginUserCommand("test@example.com", "wrong-password"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid password", result.ErrorMessage);
    }

    [Fact]
    public async Task Succeeds_WhenCredentialsValid()
    {
        var user = User.Create("TestUser", "test@example.com", "hashed-password");

        _userRepository
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(x => x.Verify("correct-password", "hashed-password"))
            .Returns(true);

        var result = await _service.ExecuteAsync(new LoginUserCommand("test@example.com", "correct-password"));

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.UserName, result.UserName);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task NormalizesEmail_BeforeLookup()
    {
        _userRepository
            .Setup(x => x.FindByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null)
            .Verifiable();

        await _service.ExecuteAsync(new LoginUserCommand("  Test@Example.com  ", "password"));

        _userRepository.Verify(x => x.FindByEmailAsync("test@example.com", It.IsAny<CancellationToken>()), Times.Once);
    }
}