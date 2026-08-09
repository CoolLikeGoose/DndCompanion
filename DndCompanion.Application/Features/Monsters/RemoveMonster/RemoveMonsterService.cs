using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.RemoveMonster;

public class RemoveMonsterService
{
    private readonly ISessionRepository _sessionRepository;
    
    public RemoveMonsterService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<RemoveMonsterResult> ExecuteAsync(
        RemoveMonsterCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            return new RemoveMonsterResult(false, $"Session with id {command.SessionId} not found.");
        }

        try
        {
            session.RemoveMonster(command.MonsterId);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new RemoveMonsterResult(true);
        }
        catch (Exception e)
        {
            return new RemoveMonsterResult(false, e.Message);
        }
    }
}