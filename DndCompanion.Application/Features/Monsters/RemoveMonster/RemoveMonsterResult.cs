namespace DndCompanion.Application.Features.Monsters.RemoveMonster;

public record RemoveMonsterResult(
    bool IsSuccess,
    string? ErrorMessage = null);