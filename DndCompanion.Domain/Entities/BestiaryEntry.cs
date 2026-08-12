namespace Domain.Entities;

public class BestiaryEntry
{
    private BestiaryEntry()
    {
    }
    
    public Guid BestiaryEntryId { get; private set; }
    public Guid MasterId { get; private set; }
    public string Name { get; private set; } = null!;
    public int MaxHp { get; private set; }
    public string? Description { get; private set; }

    public static BestiaryEntry Create(Guid masterId, string name, int maxHp, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        var normalizedName = name.Trim();
        if (normalizedName.Length > 100)
            throw new ArgumentException("Name is too long(max 100 chars)", nameof(name));

        if (maxHp <= 0)
            throw new ArgumentException("Max HP must be greater than 0", nameof(maxHp));
        
        return new BestiaryEntry
        {
            BestiaryEntryId = Guid.NewGuid(),
            MasterId = masterId,
            Name = normalizedName,
            MaxHp = maxHp,
            Description = description
        };
    }
    
    public void Update(string? name = null, int? maxHp = null, string? description = null)
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

        if (maxHp.HasValue)
            MaxHp = maxHp.Value;

        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
    }
}