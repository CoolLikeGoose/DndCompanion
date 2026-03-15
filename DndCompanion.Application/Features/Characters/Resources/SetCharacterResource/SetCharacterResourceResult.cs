using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.SetCharacterResource;

public sealed record SetCharacterResourceResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    ResourceType? ResourceType = null,
    int? Variant = null,
    int? CurrentValue = null,
    int? MaxValue = null);