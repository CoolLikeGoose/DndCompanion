namespace DndCompanion.Application.Features.Monsters.AddBestiaryEntry;

public sealed record AddBestiaryEntryResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? BestiaryEntryId = null);