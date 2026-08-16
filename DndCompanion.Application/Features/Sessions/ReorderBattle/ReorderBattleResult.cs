namespace DndCompanion.Application.Features.Sessions.ReorderBattle;

public sealed record ReorderBattleResult(
    bool IsSuccess,
    string? ErrorMessage = null);