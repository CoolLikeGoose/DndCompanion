namespace DndCompanion.Application.Features.Monsters.UpdateBestiaryEntry;

public record UpdateBestiaryEntryCommand(
    Guid BestiaryEntryId,
    string? Name = null,
    int? MaxHp = null,
    string? Description = null);