namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest.ApplyRestSingle;

public record ApplyRestSingleResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    int? AffectedResources = null);