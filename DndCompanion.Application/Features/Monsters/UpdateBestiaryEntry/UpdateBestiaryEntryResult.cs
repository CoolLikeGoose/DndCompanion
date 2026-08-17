namespace DndCompanion.Application.Features.Monsters.UpdateBestiaryEntry;

public record UpdateBestiaryEntryResult(
    bool IsSuccess,
    string? ErrorMessage = null);