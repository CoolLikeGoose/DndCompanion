using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Sessions.AddBattle;

public class AddBattleService
{
    private readonly ISessionRepository _sessionRepository;

    public AddBattleService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public async Task<AddBattleResult> ExecuteAsync(
        AddBattleCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
            return new AddBattleResult(false, $"Session with id {command.SessionId} not found.");

        try
        {
            var battle = session.AddBattle(command.Name);
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new AddBattleResult(true, Battle: battle);
        }
        catch (Exception e)
        {
            return new AddBattleResult(false, e.Message);
        }
    }
}