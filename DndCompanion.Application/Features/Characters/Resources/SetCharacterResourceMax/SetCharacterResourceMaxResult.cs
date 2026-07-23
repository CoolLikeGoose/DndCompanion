using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.SetCharacterResourceMax;

public sealed record SetCharacterResourceMaxResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    ResourceType? ResourceType = null,
    string? Name = null,
    int? CurrentValue = null,
    int? MaxValue = null);