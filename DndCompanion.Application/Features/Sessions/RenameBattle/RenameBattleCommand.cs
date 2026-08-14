namespace DndCompanion.Application.Features.Sessions.RenameBattle;

public sealed record RenameBattleCommand(
    Guid SessionId, 
    Guid BattleId, 
    string Name);