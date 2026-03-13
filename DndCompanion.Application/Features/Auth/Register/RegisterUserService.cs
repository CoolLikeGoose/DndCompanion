using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using Domain.Entities;

namespace DndCompanion.Application.Features.Auth.Register;

public sealed class RegisterUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public RegisterUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> ExecuteAsync(RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.UserName))
            return new RegisterUserResult(false, "User name is required");
        
        if (string.IsNullOrWhiteSpace(command.Email))
            return new RegisterUserResult(false, "Email is required");
        
        if (string.IsNullOrWhiteSpace(command.Password))
            return new RegisterUserResult(false, "Password is required");

        if (command.Password != command.ConfirmPassword)
            return new RegisterUserResult(false, "Passwords didnt match");

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var exists = await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken);
        if (exists)
            return new RegisterUserResult(false, "User with this email already exists");

        var passwordHash = _passwordHasher.Hash(command.Password);
        var user = User.Create(command.UserName, normalizedEmail, passwordHash);
        
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        
        return new RegisterUserResult(true, null, user.Id);
    }
}