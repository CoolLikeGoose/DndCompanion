namespace DndCompanion.Application.Features.Monsters.MoveMonsterToBattle;

public sealed record MoveMonsterToBattleCommand(
    Guid SessionId, 
    Guid MonsterId,
    Guid TargetBattleId, 
    double NewOrder);