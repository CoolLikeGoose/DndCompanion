using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Sessions.RenameBattle;

public class RenameBattleService
{
    private readonly ISessionRepository _sessionRepository;
    
    public RenameBattleService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<RenameBattleResult> ExecuteAsync(
        RenameBattleCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
            return new RenameBattleResult(false, $"Session with id {command.SessionId} not found.");

        var battle = session.Battles.FirstOrDefault(b => b.BattleId == command.BattleId);
        if (battle is null)
            return new RenameBattleResult(false, $"Battle {command.BattleId} not found.");

        try
        {
            battle.Rename(command.Name);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new RenameBattleResult(true);
        }
        catch (Exception e)
        {
            return new RenameBattleResult(false, e.Message);
        }
    }
}