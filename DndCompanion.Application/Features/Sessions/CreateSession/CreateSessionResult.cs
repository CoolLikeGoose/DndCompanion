namespace DndCompanion.Application.Features.Sessions.CreateSession;

public record CreateSessionResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    Guid? SessionId = null,
    Guid? ParticipantId = null,
    string? InviteCode = null,
    string? PinCode = null);