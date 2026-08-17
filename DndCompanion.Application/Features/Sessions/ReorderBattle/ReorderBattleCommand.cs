namespace DndCompanion.Application.Features.Sessions.ReorderBattle;

public sealed record ReorderBattleCommand(
    Guid SessionId, 
    Guid BattleId, 
    double NewOrder);