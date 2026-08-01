namespace Domain.Entities;

public class CharacterStats
{
    private CharacterStats()
    {
    }

    public Guid CharacterId { get; private set; }
    public int Strength { get; private set; }
    public int Dexterity { get; private set; }
    public int Constitution { get; private set; }
    public int Intelligence { get; private set; }
    public int Wisdom { get; private set; }
    public int Charisma { get; private set; }

    public static CharacterStats Create(
        Guid characterId)
    {
        if (characterId == Guid.Empty)
            throw new ArgumentException("CharacterId is required", nameof(characterId));

        return new CharacterStats
        {
            CharacterId = characterId,
            Strength = 10,
            Dexterity = 10,
            Constitution = 10,
            Intelligence = 10,
            Wisdom = 10,
            Charisma = 10
        };
    }

    public void Update(
        int? strength = null,
        int? dexterity = null,
        int? constitution = null,
        int? intelligence = null,
        int? wisdom = null,
        int? charisma = null)
    {
        if (strength.HasValue) Strength = strength.Value;
        if (dexterity.HasValue) Dexterity = dexterity.Value;
        if (constitution.HasValue) Constitution = constitution.Value;
        if (intelligence.HasValue) Intelligence = intelligence.Value;
        if (wisdom.HasValue) Wisdom = wisdom.Value;
        if (charisma.HasValue) Charisma = charisma.Value;
    }
}