namespace DndCompanion.Application.Features.Sessions.CreateSession;

public sealed record CreateSessionCommand(
    string? MasterDisplayName,
    string? PinCode);