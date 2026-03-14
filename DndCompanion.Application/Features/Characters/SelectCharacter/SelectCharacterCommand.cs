namespace DndCompanion.Application.Features.Characters.SelectCharacter;

public sealed record SelectCharacterCommand(
    Guid CharacterId,
    Guid SessionId,
    Guid ParticipantId);