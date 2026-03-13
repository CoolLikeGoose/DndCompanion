using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Auth.Login;

public sealed class LoginUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public LoginUserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<LoginUserResult> ExecuteAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return new LoginUserResult(false, "Email is required");
        
        if (string.IsNullOrWhiteSpace(command.Password))
            return new LoginUserResult(false, "Password is required");
        
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        
        var user = await _userRepository.FindByEmailAsync(normalizedEmail, cancellationToken);
        if (user == null)
            return new LoginUserResult(false, "User not found");
        
        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            return new LoginUserResult(false, "Invalid password");
        
        return new LoginUserResult(true, null, user.Id, user.UserName, user.Email);
    }
}