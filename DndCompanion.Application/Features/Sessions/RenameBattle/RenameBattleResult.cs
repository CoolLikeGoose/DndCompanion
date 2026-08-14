namespace DndCompanion.Application.Features.Sessions.RenameBattle;

public record RenameBattleResult(
    bool IsSuccess,
    string? ErrorMessage = null);