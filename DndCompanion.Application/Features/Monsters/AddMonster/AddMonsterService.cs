using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.AddMonster;

public class AddMonsterService
{
    private readonly ISessionRepository _sessionRepository;

    public AddMonsterService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<AddMonsterResult> ExecuteAsync(
        AddMonsterCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            return new AddMonsterResult(false, $"Session with id {command.SessionId} not found.");
        }

        try
        {
            var monster = session.AddMonster(
                command.Name,
                command.MaxHitPoints,
                command.Description);
            
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new AddMonsterResult(true, Monster: monster);
        }
        catch (Exception e)
        {
            return new AddMonsterResult(false, e.Message);
        }
    }
}