namespace DndCompanion.Application.Features.Monsters.UpdateMonster;

public record UpdateMonsterResult(
    bool IsSuccess,
    string? ErrorMessage = null);