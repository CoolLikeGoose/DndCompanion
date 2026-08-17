namespace DndCompanion.Application.Features.Monsters.RemoveMonster;

public record RemoveMonsterCommand(
    Guid SessionId,
    Guid MonsterId);