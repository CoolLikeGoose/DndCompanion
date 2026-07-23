using Domain.Enums;

namespace Domain.Entities;

public class Resource
{
    private Resource()
    {
        
    }
    
    // Resources are identified by their type and name. The ID is only used for database purposes.
    public Guid Id { get; private set; }
    public Guid CharacterId { get; private set; }
    public string? Name { get; private set; }           // Name of the resource, e.g. "Rage points"
    public string? Group { get; private set; }          // Group of the resource, e.g. "Spell Slots" for multiple spell slots of different levels
    public ResourceType Type { get; private set; }
    public int CurrentValue { get; private set; }
    public int MaxValue { get; private set; }
    public RecoveryType RecoveryType { get; private set; }
    
    public static Resource Create(
        Guid characterId, 
        string? name,
        ResourceType type, 
        int maxValue, 
        RecoveryType recoveryType,
        string? group = null,
        int? initialCurrent = null)
    {
        if (characterId == Guid.Empty) 
            throw new ArgumentException("Character ID is required", nameof(characterId));
        
        if (maxValue < 0) 
            throw new ArgumentException("Max value cannot be negative", nameof(maxValue));
        
        var currentValue = initialCurrent ?? maxValue;
        if (currentValue < 0 || currentValue > maxValue) 
            throw new ArgumentException("Initial current value must be between 0 and max value", nameof(initialCurrent));
        
        return new Resource
        {
            Id = Guid.NewGuid(),
            CharacterId = characterId,
            Name = name,
            Group = group,
            Type = type,
            CurrentValue = currentValue,
            MaxValue = maxValue,
            RecoveryType = recoveryType
        };
    }
    
    public bool MatchesType(ResourceType type, string? name = null)
    {
        return Type == type && Name == name;
    }

    public void Change(int delta)
    {
        CurrentValue = Math.Clamp(CurrentValue + delta, 0, MaxValue);
    }

    public void SetCurrent(int value)
    {
        CurrentValue = Math.Clamp(value, 0, MaxValue);
    }
    
    public void SetMax(int value, bool fillToMaxIfChanged = true)
    {
        if (value < 0) 
            throw new ArgumentException("Max value cannot be negative", nameof(value));
        
        MaxValue = value;
        if (CurrentValue > MaxValue)
            CurrentValue = MaxValue;
        
        if (fillToMaxIfChanged)
            CurrentValue = MaxValue;
    }
    
    // TODO: Change includeShortOnLongRest to a more flexible system of recovery rules
    public bool CanRecoverOn(RecoveryType restType, bool includeShortOnLongRest = true)
    {
        if (RecoveryType == RecoveryType.None)
            return false;
        
        if (RecoveryType == restType)
            return true;
        
        if (RecoveryType == RecoveryType.ShortRest && restType == RecoveryType.LongRest && includeShortOnLongRest)
            return true;
        
        return false;
    }

    public void RecoverToMax()
    {
        CurrentValue = MaxValue;
    }
}