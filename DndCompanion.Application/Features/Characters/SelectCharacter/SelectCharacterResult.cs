namespace DndCompanion.Application.Features.Characters.SelectCharacter;

public sealed record SelectCharacterResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? SessionId = null,
    Guid? ParticipantId = null);