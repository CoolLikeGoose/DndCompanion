namespace Domain.Entities;

public class Item
{
    private Item()
    {
        
    }
    
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? SourceUrl { get; private set; }
    public int Quantity { get; private set; }
    public Guid? ChargeResourceId { get; private set; }
    
    public static Item Create(
        Guid characterId,
        string name,
        string? description = null,
        string? sourceUrl = null,
        int quantity = 1,
        Guid? chargeResourceId = null)
    {
        if (characterId == Guid.Empty) 
            throw new ArgumentException("CharacterId is required", nameof(characterId));
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Name is required", nameof(name));

        var normalizedName = name.Trim();
        if (normalizedName.Length > 100) 
            throw new ArgumentException("Name is too long(max 100 chars)", nameof(name));

        if (quantity < 0)
            throw new ArgumentException("Quantity must be a positive number", nameof(quantity));

        return new Item
        {
            Id = Guid.NewGuid(),
            CharacterId = characterId,
            Name = normalizedName,
            Description = description?.Trim(),
            SourceUrl = sourceUrl?.Trim(),
            Quantity = quantity
        };
    }
    
    public void AssignChargeResource(Guid chargeResourceId)
    {
        if (chargeResourceId == Guid.Empty)
            throw new ArgumentException("Charge resource id is required", nameof(chargeResourceId));
        ChargeResourceId = chargeResourceId;
    }

    public void Update(
        string? name = null,
        string? description = null,
        string? sourceUrl = null,
        int? quantity = null)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentException("Name cannot be empty", nameof(name));

            var normalizedName = name.Trim();
            if (normalizedName.Length > 100) 
                throw new ArgumentException("Name is too long(max 100 chars)", nameof(name));
            
            Name = normalizedName;
        }
        
        if (description is not null)
        {
            Description = description.Trim();
        }
        
        if (sourceUrl is not null)
        {
            SourceUrl = sourceUrl.Trim();
        }
        
        if (quantity is not null)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity must be a positive number", nameof(quantity));
            Quantity = quantity.Value;
        }
    }
}