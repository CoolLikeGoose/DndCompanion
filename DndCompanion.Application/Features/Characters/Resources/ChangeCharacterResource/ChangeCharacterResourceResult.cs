using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.ChangeCharacterResource;

public sealed record ChangeCharacterResourceResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    ResourceType? ResourceType = null,
    int? Variant = null,
    int? CurrentValue = null,
    int? MaxValue = null);