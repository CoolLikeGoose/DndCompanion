namespace DndCompanion.Application.Features.Monsters.ReorderMonster;

public sealed record ReorderMonsterResult(
    bool IsSuccess,
    string? ErrorMessage = null);