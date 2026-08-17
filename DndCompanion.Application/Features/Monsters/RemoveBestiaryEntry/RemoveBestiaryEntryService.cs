using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.RemoveBestiaryEntry;

public class RemoveBestiaryEntryService
{
    private readonly IBestiaryRepository _repository;
    private readonly ICurrentUser _currentUser;

    public RemoveBestiaryEntryService(IBestiaryRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<RemoveBestiaryEntryResult> ExecuteAsync(
        RemoveBestiaryEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.FindByIdAsync(command.BestiaryEntryId, cancellationToken);
        if (entry is null)
            return new RemoveBestiaryEntryResult(false, $"Bestiary entry {command.BestiaryEntryId} not found.");
        
        if (entry.MasterId != _currentUser.UserId)
            return new RemoveBestiaryEntryResult(false, $"Bestiary entry {command.BestiaryEntryId} is not owned by user {_currentUser.UserId}.");

        _repository.Remove(entry);
        await _repository.SaveChangesAsync(cancellationToken);
        return new RemoveBestiaryEntryResult(true);
    }
}