namespace Domain.Entities;

public class Monster
{
    private Monster()
    {
    }

    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string Name { get; private set; } = null!;

    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }
    public string? Description { get; private set; }

    public static Monster Create(Guid sessionId, string name, int maxHp, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        var normalizedName = name.Trim();
        if (normalizedName.Length > 100)
            throw new ArgumentException("Name is too long(max 100 chars)", nameof(name));

        if (maxHp <= 0)
            throw new ArgumentException("Max HP must be greater than 0", nameof(maxHp));

        var monster = new Monster
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            Name = normalizedName,
            MaxHp = maxHp,
            CurrentHp = maxHp,
            Description = description
        };

        return monster;
    }

    public void Update(
        string? name = null, 
        int? maxHp = null, 
        int? currentHp = null, 
        string? description = null)
    {
        if (maxHp.HasValue && maxHp <= 0)
            throw new ArgumentException("Max HP must be greater than 0", nameof(maxHp));
        
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty string", nameof(name));
            
            var normalizedName = name.Trim();
            if (normalizedName.Length > 100)
                throw new ArgumentException("Name is too long(max 100 chars)", nameof(name));
            
            Name = normalizedName;
        }
        
        MaxHp = maxHp.HasValue ? maxHp.Value : MaxHp;
        CurrentHp = currentHp.HasValue ? Math.Clamp(currentHp.Value, 0, MaxHp) : Math.Min(CurrentHp, MaxHp);
        Description = description ?? Description;
    }
}