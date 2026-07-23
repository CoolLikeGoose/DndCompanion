using Domain.Enums;

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
    
    private readonly List<Resource> _resources = new();
    public IReadOnlyCollection<Resource> Resources => _resources.AsReadOnly();
    
    public static Character Create(string name, Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Name is required", nameof(name));

        var normalizedName = name.Trim();
        if (normalizedName.Length > 100) 
            throw new ArgumentException("Name is too long(max 100 chars)", nameof(name));

        return new Character
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddResource(
        ResourceType type, 
        int maximum, 
        RecoveryType recovery,
        string? name = null,
        string? group = null,
        int? initialCurrent = null)
    {
        if (_resources.Any(x => x.MatchesType(type, name)))
            throw new InvalidOperationException($"Resource of type {type} with name {name ?? "NONE"} already exists for this character.");
        
        var resource = Resource.Create(Id, name, type, maximum, recovery, group, initialCurrent);
        _resources.Add(resource);   
    }
    
    public Resource ChangeResource(ResourceType type, string? name, int delta)
    {
        var resource = GetResource(type, name);
        resource.Change(delta);
        return resource;
    }
    
    public Resource SetResource(ResourceType type, string? name, int value)
    {
        var resource = GetResource(type, name);
        resource.SetCurrent(value);
        return resource;
    }

    public Resource SetResourceMaximum(ResourceType type, string? name, int maximum, bool fillToMaxIfReduced = true)
    {
        var resource = GetResource(type, name);
        resource.SetMax(maximum, fillToMaxIfReduced);
        return resource;
    }

    public int ApplyRest(RecoveryType restType, bool includeShortOnLongRest = true)
    {
        var affected = 0;
        foreach (var resource in _resources.Where(x => x.CanRecoverOn(restType, includeShortOnLongRest)))
        {
            resource.RecoverToMax();
            affected++;
        }

        return affected;
    }

    private Resource GetResource(ResourceType type, string? name)
    {
        var resource = _resources.FirstOrDefault(x => x.MatchesType(type, name));
        if (resource is null)
            throw new ArgumentException($"Resource of type {type} with name {name ?? "NONE"} not found for this character.", nameof(type));
        
        return resource;
    }
}