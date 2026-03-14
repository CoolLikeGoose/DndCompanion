using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using Domain.ValueObjects;

namespace DndCompanion.Application.Features.Sessions.JoinSession;

public sealed class JoinSessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICurrentUser _currentUser;
    
    public JoinSessionService(ISessionRepository sessionRepository, ICurrentUser currentUser)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
    }

    public async Task<JoinSessionResult> ExecuteAsync(
        JoinSessionCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.InviteCode))
            return new JoinSessionResult(false, "Invite code is required");

        var displayName = _currentUser.IsAuthenticated
            ? _currentUser.UserName
            : command.DisplayName;

        if (string.IsNullOrWhiteSpace(displayName))
            return new JoinSessionResult(false, "Display name is required");

        var session = await _sessionRepository.FindByInviteCodeAsync(command.InviteCode, cancellationToken);
        if (session is null)
            return new JoinSessionResult(false, "Session not found");
        
        PinCode? pinCode = null;
        if (!string.IsNullOrWhiteSpace(command.PinCode))
        {
            try
            {
                pinCode = PinCode.From(command.PinCode);
            }
            catch (ArgumentException e)
            {
                return new JoinSessionResult(false, e.Message);
            }
        }

        try
        {
            var participant = session.Join(displayName, _currentUser.UserId, pinCode);

            if (_currentUser.UserId is { } currentUserId)
                await _sessionRepository.RemoveParticipantsByUserIdAsync(currentUserId, session.Id, cancellationToken);
            
            await _sessionRepository.AddParticipantAsync(participant, cancellationToken);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            
            return new JoinSessionResult(true, null, session.Id, participant.Id);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return new JoinSessionResult(false, e.Message);
        }
    }
}