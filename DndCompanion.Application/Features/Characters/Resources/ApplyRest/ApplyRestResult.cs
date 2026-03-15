namespace DndCompanion.Application.Features.Characters.Resources.ApplyRest;

public record ApplyRestResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    int? AffectedResources = null);