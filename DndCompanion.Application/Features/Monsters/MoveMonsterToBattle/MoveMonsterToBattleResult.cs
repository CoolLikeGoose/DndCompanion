namespace DndCompanion.Application.Features.Monsters.MoveMonsterToBattle;

public record MoveMonsterToBattleResult(
    bool IsSuccess,
    string? ErrorMessage = null);