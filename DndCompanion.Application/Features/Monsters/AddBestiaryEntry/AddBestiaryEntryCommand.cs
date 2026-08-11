namespace DndCompanion.Application.Features.Monsters.AddBestiaryEntry;

public sealed record AddBestiaryEntryCommand(
    Guid MasterId,
    string Name,
    int MaxHitPoints,
    string? Description = null);