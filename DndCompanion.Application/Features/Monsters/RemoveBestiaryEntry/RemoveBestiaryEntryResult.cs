namespace DndCompanion.Application.Features.Monsters.RemoveBestiaryEntry;

public record RemoveBestiaryEntryResult(
    bool IsSuccess,
    string? ErrorMessage = null);