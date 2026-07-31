namespace Domain.Entities;

public class CharacterInfo
{
    private CharacterInfo()
    {
    }

    public Guid CharacterId { get; private set; }
    public string? Class { get; private set; }
    public int? Level { get; private set; }
    public string? Race { get; private set; }
    public int? Age { get; private set; }
    public string? Background { get; private set; }
    public string? Alignment { get; private set; }
    public int? ExperiencePoints { get; private set; }
    public string? PersonalityTraits { get; private set; }
    public string? Ideals { get; private set; }
    public string? Bonds { get; private set; }
    public string? Flaws { get; private set; }
    public string? LanguageProficiencies { get; private set; }
    public string? ToolProficiencies { get; private set; }

    public static CharacterInfo Create(
        Guid characterId)
    {
        if (characterId == Guid.Empty)
            throw new ArgumentException("CharacterId is required", nameof(characterId));

        return new CharacterInfo
        {
            CharacterId = characterId
        };
    }

    public void Update(
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
        if (characterClass != null) Class = characterClass.Trim();
        if (level.HasValue) Level = level.Value;
        if (race != null) Race = race.Trim();
        if (age.HasValue) Age = age.Value;
        if (background != null) Background = background.Trim();
        if (alignment != null) Alignment = alignment.Trim();
        if (experiencePoints.HasValue) ExperiencePoints = experiencePoints.Value;
        if (personalityTraits != null) PersonalityTraits = personalityTraits.Trim();
        if (ideals != null) Ideals = ideals.Trim();
        if (bonds != null) Bonds = bonds.Trim();
        if (flaws != null) Flaws = flaws.Trim();
        if (languageProficiencies != null) LanguageProficiencies = languageProficiencies.Trim();
        if (toolProficiencies != null) ToolProficiencies = toolProficiencies.Trim();
    }
}