using Domain.Entities;

namespace DndCompanion.Application.Features.Sessions.RemoveBattle;

public record RemoveBattleResult(
    bool IsSuccess,
    string? ErrorMessage = null);