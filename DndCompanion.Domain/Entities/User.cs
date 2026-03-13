namespace Domain.Entities;

public class User
{
    private User()
    {
        
    }
    
    public Guid Id { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Email { get; private set; } = null!; 
    public string PasswordHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    public static User Create(string userName, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name is required", nameof(userName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password is required", nameof(passwordHash));

        return new User
        {
            Id = Guid.NewGuid(),
            UserName = userName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };
    }
}