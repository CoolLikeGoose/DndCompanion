using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Sessions.ReorderBattle;

public class ReorderBattleService
{
    private readonly ISessionRepository _sessionRepository;
    
    public ReorderBattleService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<ReorderBattleResult> ExecuteAsync(
        ReorderBattleCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
            return new ReorderBattleResult(false, $"Session with id {command.SessionId} not found.");

        try
        {
            session.ReorderBattle(command.BattleId, command.NewOrder);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new ReorderBattleResult(true);
        }
        catch (Exception e)
        {
            return new ReorderBattleResult(false, e.Message);
        }
    }
}