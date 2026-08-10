namespace DndCompanion.Application.Features.Monsters.UpdateMonster;

public record UpdateMonsterCommand(
    Guid SessionId,
    Guid MonsterId,
    string? Name = null,
    int? MaxHitPoints = null,
    int? HitPoints = null,
    string? Description = null);