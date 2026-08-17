namespace DndCompanion.Application.Features.Monsters.ReorderMonster;

public sealed record ReorderMonsterCommand(
    Guid SessionId, 
    Guid MonsterId, 
    double NewOrder);