using DndCompanion.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _passwordHasher = new(); 
    
    public string Hash(string password)
    {
        return _passwordHasher.HashPassword(new object(), password);
    }

    public bool Verify(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(new object(), hashedPassword, password);
        return result != PasswordVerificationResult.Failed;
    }
}