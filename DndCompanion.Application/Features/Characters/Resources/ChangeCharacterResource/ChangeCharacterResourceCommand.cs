using Domain.Enums;

namespace DndCompanion.Application.Features.Characters.Resources.ChangeCharacterResource;

public sealed record ChangeCharacterResourceCommand(
    Guid ParticipantId,
    ResourceType ResourceType,
    string? Name,
    int Delta);