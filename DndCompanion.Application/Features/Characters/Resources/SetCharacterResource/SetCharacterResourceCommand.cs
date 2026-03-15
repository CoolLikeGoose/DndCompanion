using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.SetCharacterResource;

public sealed record SetCharacterResourceCommand(
    Guid ParticipantId,
    ResourceType ResourceType,
    int? Variant,
    int Value);