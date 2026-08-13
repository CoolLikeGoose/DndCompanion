namespace DndCompanion.Application.Features.Sessions.AddBattle;

public sealed record AddBattleCommand(
    Guid SessionId, 
    string Name);