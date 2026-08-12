using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.AddBestiaryEntry;

namespace DndCompanion.Application.Features.Monsters.AddMonster;

public class AddMonsterService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly AddBestiaryEntryService _addBestiaryEntryService;
    private readonly ICurrentUser _currentUser;

    public AddMonsterService(ISessionRepository sessionRepository,
        AddBestiaryEntryService addBestiaryEntryService,
        ICurrentUser CurrentUser)
    {
        _sessionRepository = sessionRepository;
        _addBestiaryEntryService = addBestiaryEntryService;
        _currentUser = CurrentUser;
    }

    public async Task<AddMonsterResult> ExecuteAsync(
        AddMonsterCommand command, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.FindByIdWithMonstersAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            return new AddMonsterResult(false, $"Session with id {command.SessionId} not found.");
        }
        
        AddBestiaryEntryResult? bestiaryEntryResult = null;
        if (command.SaveToBestiary && _currentUser.UserId.HasValue)
        {
            bestiaryEntryResult = await _addBestiaryEntryService.ExecuteAsync(new AddBestiaryEntryCommand(
                _currentUser.UserId.Value,
                command.Name,
                command.MaxHitPoints,
                command.Description));
        };

        try
        {
            var monster = session.AddMonster(
                command.Name,
                command.MaxHitPoints,
                command.Description,
                bestiaryEntryResult?.BestiaryEntryId);
            
            // saves twice per operation if addBestiaryEntry is called, but this is acceptable for now
            await _sessionRepository.SaveChangesAsync(cancellationToken);
            return new AddMonsterResult(true, Monster: monster);
        }
        catch (Exception e)
        {
            return new AddMonsterResult(false, e.Message);
        }
    }
}