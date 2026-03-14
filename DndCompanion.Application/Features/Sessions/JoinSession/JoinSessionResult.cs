namespace DndCompanion.Application.Features.Sessions.JoinSession;

public record JoinSessionResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? SessionId = null,
    Guid? ParticipantId = null);