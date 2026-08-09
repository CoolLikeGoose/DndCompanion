namespace DndCompanion.Application.Features.Monsters.UpdateMonster;

public record UpdateMonsterCommand(
    Guid SessionId,
    Guid MonsterId,
    string Name,
    int MaxHitPoints,
    int? HitPoints = null,
    string? Description = null);