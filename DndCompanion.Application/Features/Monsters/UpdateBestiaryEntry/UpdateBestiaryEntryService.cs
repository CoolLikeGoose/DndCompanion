using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.UpdateBestiaryEntry;

public class UpdateBestiaryEntryService
{
    private readonly IBestiaryRepository _repository;
    private readonly ICurrentUser _currentUser;

    public UpdateBestiaryEntryService(IBestiaryRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<UpdateBestiaryEntryResult> ExecuteAsync(
        UpdateBestiaryEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.FindByIdAsync(command.BestiaryEntryId, cancellationToken);
        if (entry is null)
            return new UpdateBestiaryEntryResult(false, $"Bestiary entry {command.BestiaryEntryId} not found.");
        
        if (entry.MasterId != _currentUser.UserId)
            return new UpdateBestiaryEntryResult(false, $"Bestiary entry {command.BestiaryEntryId} is not owned by user {_currentUser.UserId}.");
        
        try
        {
            entry.Update(command.Name, command.MaxHp, command.Description);
            await _repository.SaveChangesAsync(cancellationToken);
            return new UpdateBestiaryEntryResult(true);
        }
        catch (Exception e)
        {
            return new UpdateBestiaryEntryResult(false, e.Message);
        }
    }
}