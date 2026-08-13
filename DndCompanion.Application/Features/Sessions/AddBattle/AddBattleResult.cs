using Domain.Entities;

namespace DndCompanion.Application.Features.Sessions.AddBattle;

public record AddBattleResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Battle? Battle = null);