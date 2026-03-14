namespace DndCompanion.Application.Features.Sessions.JoinSession;

public sealed record JoinSessionCommand(
    string InviteCode,
    string DisplayName,
    string? PinCode);