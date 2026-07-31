namespace DndCompanion.Application.Features.Characters.UpdateStats;

public record UpdateStatsCommand(
    Guid ParticipantId,
    int? Strength = null,
    int? Dexterity = null,
    int? Constitution = null,
    int? Intelligence = null,
    int? Wisdom = null,
    int? Charisma = null);