using DndCompanion.Application.Abstractions.Persistence;

namespace DndCompanion.Application.Features.Monsters.UpdateBestiaryEntry;

public class UpdateBestiaryEntryService
{
    private readonly IBestiaryRepository _repository;

    public UpdateBestiaryEntryService(IBestiaryRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateBestiaryEntryResult> ExecuteAsync(
        UpdateBestiaryEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await _repository.FindByIdAsync(command.BestiaryEntryId, cancellationToken);
        if (entry is null)
            return new UpdateBestiaryEntryResult(false, $"Bestiary entry {command.BestiaryEntryId} not found.");
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