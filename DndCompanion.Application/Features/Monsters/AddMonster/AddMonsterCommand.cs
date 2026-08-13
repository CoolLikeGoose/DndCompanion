namespace DndCompanion.Application.Features.Monsters.AddMonster;

public record AddMonsterCommand(
    Guid SessionId,
    string Name,
    int MaxHitPoints,
    string? Description = null,
    bool SaveToBestiary = false,
    Guid? BattleId = null);
