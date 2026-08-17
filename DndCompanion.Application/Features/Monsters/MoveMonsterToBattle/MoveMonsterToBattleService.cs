using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.MoveMonsterToBattle;

public class MoveMonsterToBattleService
{
    private readonly ISessionRepository _sessionRepository;
    
    public MoveMonsterToBattleService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<MoveMonsterToBattleResult> ExecuteAsync(
        MoveMonsterToBattleCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
            return new MoveMonsterToBattleResult(false, $"Session with id {command.SessionId} not found.");

        try
        {
            session.MoveMonsterToBattle(command.MonsterId, command.TargetBattleId, command.NewOrder);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new MoveMonsterToBattleResult(true);
        }
        catch (Exception e)
        {
            return new MoveMonsterToBattleResult(false, e.Message);
        }
    }
}