namespace Web.Services;

public sealed record SessionEvent( 
    SessionEventType Type,
    Guid SessionId,
    Guid? ParticipantId = null,
    BattleLogEntry? LogEntry = null);