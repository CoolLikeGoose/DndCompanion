using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.SetCharacterResourceMax;

public sealed record SetCharacterResourceMaxCommand(
    Guid ParticipantId,
    ResourceType ResourceType,
    int? Variant,
    int Value);