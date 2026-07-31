namespace DndCompanion.Application.Features.Characters.UpdateInfo;

public record UpdateInfoResult(
    bool IsSuccess,
    string? ErrorMessage = null);