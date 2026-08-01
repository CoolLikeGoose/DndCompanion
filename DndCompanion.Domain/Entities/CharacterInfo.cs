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
        static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        
        if (characterClass != null) Class = Normalize(characterClass);
        if (level.HasValue) Level = level.Value;
        if (race != null) Race = Normalize(race);
        if (age.HasValue) Age = age.Value;
        if (background != null) Background = Normalize(background);
        if (alignment != null) Alignment = Normalize(alignment);
        if (experiencePoints.HasValue) ExperiencePoints = experiencePoints.Value;
        if (personalityTraits != null) PersonalityTraits = Normalize(personalityTraits);
        if (ideals != null) Ideals = Normalize(ideals);
        if (bonds != null) Bonds = Normalize(bonds);
        if (flaws != null) Flaws = Normalize(flaws);
        if (languageProficiencies != null) LanguageProficiencies = Normalize(languageProficiencies);
        if (toolProficiencies != null) ToolProficiencies = Normalize(toolProficiencies);
    }
}