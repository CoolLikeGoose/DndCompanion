namespace DndCompanion.Application.Features.Characters.Resources.AddDeathSave;

public sealed record AddDeathSaveResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    int? Successes = null,
    int? Failures = null);