using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.ReorderMonster;

public class ReorderMonsterService
{
    private readonly ISessionRepository _sessionRepository;
    
    public ReorderMonsterService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<ReorderMonsterResult> ExecuteAsync(
        ReorderMonsterCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
            return new ReorderMonsterResult(false, $"Session with id {command.SessionId} not found.");

        try
        {
            session.ReorderMonster(command.MonsterId, command.NewOrder);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new ReorderMonsterResult(true);
        }
        catch (Exception e)
        {
            return new ReorderMonsterResult(false, e.Message);
        }
    }
}