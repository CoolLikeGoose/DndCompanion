using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.ValueObjects;

namespace DndCompanion.Application.Features.Sessions.CreateSession;

public sealed class CreateSessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ICurrentUser _currentUser;
    
    public CreateSessionService(ISessionRepository sessionRepository, ICurrentUser currentUser)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
    }

    public async Task<CreateSessionResult> ExecuteAsync(
        CreateSessionCommand command, CancellationToken cancellationToken = default)
    {
        var masterName = _currentUser.IsAuthenticated ? _currentUser.UserName : command.MasterDisplayName;

        if (string.IsNullOrWhiteSpace(masterName))
            return new CreateSessionResult(false, "Master name is required");

        PinCode? pinCode = null;
        if (!string.IsNullOrWhiteSpace(command.PinCode))
        {
            try
            {
                pinCode = PinCode.From(command.PinCode);
            }
            catch (ArgumentException e)
            {
                return new CreateSessionResult(false, e.Message);
            }
        }

        Session session;
        try
        {
            session = Session.Create(_currentUser.UserId, masterName, pinCode);
        }
        catch (ArgumentException e)
        {
            return new CreateSessionResult(false, e.Message);
        }

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);
        
        return new CreateSessionResult(
            true, 
            null, 
            session.Id, 
            session.InviteCode.Value,
            session.PinCode?.Value);
    }
}