namespace Domain.Entities;

public class Character
{
    private Character()
    {
        
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid? UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Character Create(string name, Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Name is required", nameof(name));

        var normalizedName = name.Trim();
        if (normalizedName.Length > 100) 
            throw new ArgumentException("Name is too ling(max 100 chars)", nameof(name));

        return new Character
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }
}