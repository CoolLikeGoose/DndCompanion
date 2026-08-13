using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.RemoveBestiaryEntry;

public class RemoveBestiaryEntryService
{
    private readonly IBestiaryRepository _repository;

    public RemoveBestiaryEntryService(IBestiaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<RemoveBestiaryEntryResult> ExecuteAsync(
        RemoveBestiaryEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.FindByIdAsync(command.BestiaryEntryId, cancellationToken);
        if (entry is null)
            return new RemoveBestiaryEntryResult(false, $"Bestiary entry {command.BestiaryEntryId} not found.");

        _repository.Remove(entry);
        await _repository.SaveChangesAsync(cancellationToken);
        return new RemoveBestiaryEntryResult(true);
    }
}