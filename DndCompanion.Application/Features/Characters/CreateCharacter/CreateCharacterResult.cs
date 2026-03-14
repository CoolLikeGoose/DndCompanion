namespace DndCompanion.Application.Features.Characters.CreateCharacter;

public sealed record CreateCharacterResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? CharacterId = null,
    Guid? SessionId = null,
    Guid? ParticipantId = null);