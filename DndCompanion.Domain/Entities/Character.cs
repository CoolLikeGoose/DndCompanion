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

    public int DeathSavesSuccesses { get; private set; }
    public int DeathSavesFailures { get; private set; }

    private readonly List<Resource> _resources = new();
    public IReadOnlyCollection<Resource> Resources => _resources.AsReadOnly();

    private readonly List<Item> _items = new();
    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    private CharacterInfo _info = null!;
    public CharacterInfo Info => _info;

    private CharacterStats _stats = null!;
    public CharacterStats Stats => _stats;


    public static Character Create(string name, Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        var normalizedName = name.Trim();
        if (normalizedName.Length > 100)
            throw new ArgumentException("Name is too long(max 100 chars)", nameof(name));

        var character = new Character
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        character._info = CharacterInfo.Create(character.Id);
        character._stats = CharacterStats.Create(character.Id);

        return character;
    }

    // Resources
    public void AddResource(
        ResourceType type,
        int maximum,
        RecoveryType recovery,
        string name,
        string? group = null,
        int? initialCurrent = null)
    {
        if (_resources.Any(x => x.MatchesType(type, name)))
            throw new InvalidOperationException(
                $"Resource of type {type} with name {name ?? "NONE"} already exists for this character.");

        var resource = Resource.Create(Id, name, type, maximum, recovery, group, initialCurrent);
        _resources.Add(resource);
    }

    public Resource ChangeResource(ResourceType type, string name, int delta)
    {
        var resource = GetResource(type, name);
        
        if (type == ResourceType.HitPoints && delta > 0 && resource.CurrentValue == 0)
        {
            ResetDeathSaves();
        }
        
        resource.Change(delta);
        return resource;
    }

    public Resource SetResource(ResourceType type, string name, int value)
    {
        var resource = GetResource(type, name);
        
        if (type == ResourceType.HitPoints && value > 0 && resource.CurrentValue == 0)
        {
            ResetDeathSaves();
        }
        
        resource.SetCurrent(value);
        return resource;
    }

    public Resource SetResourceMaximum(ResourceType type, string name, int maximum, bool fillToMaxIfReduced = true)
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
            throw new ArgumentException(
                $"Resource of type {type} with name {name ?? "NONE"} not found for this character.", nameof(type));

        return resource;
    }

    public void RemoveResource(ResourceType type, string name)
    {
        var resource = _resources.FirstOrDefault(x => x.MatchesType(type, name));
        if (resource is null)
            throw new ArgumentException(
                $"Resource of type {type} with name {name} not found for this character.");
        _resources.Remove(resource);
    }

    // Items   
    public Item AddItem(
        string name,
        string? description,
        string? sourceUrl,
        int quantity = 1)
    {
        var item = Item.Create(Id, name, description, sourceUrl, quantity);
        _items.Add(item);
        return item;
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
            throw new ArgumentException($"Item with id {itemId} not found for this character.", nameof(itemId));

        _items.Remove(item);
    }

    public void UpdateItem(Guid itemId, string? name, string? description, string? sourceUrl, int? quantity)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
            throw new ArgumentException($"Item with id {itemId} not found for this character.", nameof(itemId));

        item.Update(name, description, sourceUrl, quantity);
    }

    // Stats
    public void UpdateStats(
        int? strength = null,
        int? dexterity = null,
        int? constitution = null,
        int? intelligence = null,
        int? wisdom = null,
        int? charisma = null)
    {
        _stats.Update(strength, dexterity, constitution, intelligence, wisdom, charisma);
    }

    public void UpdateInfo(
        string? characterClass = null,
        int? level = null,
        string? race = null,
        int? age = null,
        string? background = null,
        string? alignment = null,
        int? experiencePoints = null,
        string? personalityTraits = null,
        string? ideals = null,
        string? bonds = null,
        string? flaws = null,
        string? languageProficiencies = null,
        string? toolProficiencies = null)
    {
        _info.Update(characterClass, level, race, age, background, alignment, experiencePoints, personalityTraits,
            ideals, bonds, flaws, languageProficiencies, toolProficiencies);
    }

    // Death Saves
    public void AddDeathSave(bool isSuccess)
    {
        if (isSuccess)
            DeathSavesSuccesses = Math.Min(DeathSavesSuccesses + 1, 3);
        else
            DeathSavesFailures = Math.Min(DeathSavesFailures + 1, 3);
    }

    private void ResetDeathSaves() => (DeathSavesSuccesses, DeathSavesFailures) = (0, 0);
}