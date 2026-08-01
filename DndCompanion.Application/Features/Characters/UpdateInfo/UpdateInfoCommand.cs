namespace DndCompanion.Application.Features.Characters.UpdateInfo;

public sealed record UpdateInfoCommand(
    Guid ParticipantId,
    string? CharacterClass = null,
    int? Level = null,
    string? Race = null,
    int? Age = null,
    string? Background = null,
    string? Alignment = null,
    int? ExperiencePoints = null,
    string? PersonalityTraits = null,
    string? Ideals = null,
    string? Bonds = null,
    string? Flaws = null,
    string? LanguageProficiencies = null,
    string? ToolProficiencies = null);