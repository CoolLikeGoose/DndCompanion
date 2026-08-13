namespace DndCompanion.Application.Features.Sessions.RemoveBattle;

public sealed record RemoveBattleCommand(
    Guid SessionId, 
    Guid BattleId);