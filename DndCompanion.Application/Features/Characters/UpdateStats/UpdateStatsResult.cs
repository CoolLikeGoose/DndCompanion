namespace DndCompanion.Application.Features.Characters.UpdateStats;

public record UpdateStatsResult(
    bool IsSuccess,
    string? ErrorMessage = null);