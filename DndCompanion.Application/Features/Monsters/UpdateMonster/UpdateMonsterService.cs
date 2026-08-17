using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.UpdateMonster;

public class UpdateMonsterService
{
    private readonly ISessionRepository _sessionRepository;
    
    public UpdateMonsterService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<UpdateMonsterResult> ExecuteAsync(
        UpdateMonsterCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            return new UpdateMonsterResult(false, $"Session with id {command.SessionId} not found.");
        }

        try
        {
            session.UpdateMonster(
                command.MonsterId,
                command.Name,
                command.MaxHitPoints,
                command.HitPoints,
                command.Description);
            
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new UpdateMonsterResult(true);
        }
        catch (Exception e)
        {
            return new UpdateMonsterResult(false, e.Message);
        }
    }
}