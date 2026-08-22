using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Auth.Register;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Auth;

public class RegisterUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly RegisterUserService _service;

    public RegisterUserServiceTests()
    {
        _service = new RegisterUserService(_userRepository.Object, _passwordHasher.Object);

        _userRepository
            .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasher
            .Setup(x => x.Hash(It.IsAny<string>()))
            .Returns("hashed-password");
    }

    [Fact]
    public async Task Fails_WhenUserNameEmpty()
    {
        var result = await _service.ExecuteAsync(new RegisterUserCommand("", "test@example.com", "pass", "pass"));
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenEmailEmpty()
    {
        var result = await _service.ExecuteAsync(new RegisterUserCommand("TestUser", "", "pass", "pass"));
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenPasswordEmpty()
    {
        var result = await _service.ExecuteAsync(new RegisterUserCommand("TestUser", "test@example.com", "", ""));
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenPasswordsDontMatch()
    {
        var result = await _service.ExecuteAsync(
            new RegisterUserCommand("TestUser", "test@example.com", "pass1", "pass2"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords didnt match", result.ErrorMessage);
    }

    [Fact]
    public async Task Fails_WhenEmailAlreadyExists()
    {
        _userRepository
            .Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.ExecuteAsync(
            new RegisterUserCommand("TestUser", "test@example.com", "pass", "pass"));

        Assert.False(result.IsSuccess);
        Assert.Equal("User with this email already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task Succeeds_WhenValid()
    {
        var result = await _service.ExecuteAsync(
            new RegisterUserCommand("TestUser", "test@example.com", "pass", "pass"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.UserId);
    }

    [Fact]
    public async Task PersistsUser_WhenValid()
    {
        await _service.ExecuteAsync(new RegisterUserCommand("TestUser", "test@example.com", "pass", "pass"));

        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HashesPassword_BeforeStoring()
    {
        await _service.ExecuteAsync(new RegisterUserCommand("TestUser", "test@example.com", "plain-password", "plain-password"));

        _passwordHasher.Verify(x => x.Hash("plain-password"), Times.Once);
    }

    [Fact]
    public async Task NormalizesEmail_BeforeCheckingExistenceAndStoring()
    {
        await _service.ExecuteAsync(
            new RegisterUserCommand("TestUser", "  Test@Example.com  ", "pass", "pass"));

        _userRepository.Verify(x => x.ExistsByEmailAsync("test@example.com", It.IsAny<CancellationToken>()), Times.Once);
        _userRepository.Verify(
            x => x.AddAsync(It.Is<User>(u => u.Email == "test@example.com"), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}