namespace Domain.Entities;

public class Battle
{
    public Guid BattleId { get; private set; }
    public Guid SessionId { get; private set; }
    public double Order { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Battle() { }

    public static Battle Create(Guid sessionId, string name, double order)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ArgumentException("Name is required", nameof(name));

        if (normalizedName.Length > 100)
            throw new ArgumentException("Name is too long (max 100 chars)", nameof(name));

        return new Battle
        {
            BattleId = Guid.NewGuid(),
            SessionId = sessionId,
            Name = normalizedName,
            Order = order
        };
    }

    public void Rename(string name)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ArgumentException("Name is required", nameof(name));
        
        if (normalizedName.Length > 100)
            throw new ArgumentException("Name is too long (max 100 chars)", nameof(name));


        Name = normalizedName;
    }
    
    public void SetOrder(double order)
    {
        Order = order;
    }
}