using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Sessions.RemoveBattle;

public class RemoveBattleService
{
    private readonly ISessionRepository _sessionRepository;

    public RemoveBattleService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<RemoveBattleResult> ExecuteAsync(
        RemoveBattleCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
            return new RemoveBattleResult(false, $"Session with id {command.SessionId} not found.");

        try
        {
            session.RemoveBattle(command.BattleId);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new RemoveBattleResult(true);
        }
        catch (Exception e)
        {
            return new RemoveBattleResult(false, e.Message);
        }
    }
}