namespace DndCompanion.Application.Features.Characters.CreateCharacter;

public sealed record CreateCharacterCommand(
    string Name,
    Guid SessionId,
    Guid ParticipantId);