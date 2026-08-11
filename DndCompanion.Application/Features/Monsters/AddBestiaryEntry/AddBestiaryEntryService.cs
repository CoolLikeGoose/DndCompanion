using DndCompanion.Application.Abstractions.Persistence;
using Domain.Entities;

namespace DndCompanion.Application.Features.Monsters.AddBestiaryEntry;

public class AddBestiaryEntryService
{
    private readonly IBestiaryRepository _bestiaryRepository;

    public AddBestiaryEntryService(IBestiaryRepository bestiaryRepository)
    {
        _bestiaryRepository = bestiaryRepository;
    }

    public async Task<AddBestiaryEntryResult> ExecuteAsync(AddBestiaryEntryCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _bestiaryRepository.GetBestiaryEntriesAsync(command.MasterId, cancellationToken);
        if (existing.Any(x => x.Name == command.Name))
            return new AddBestiaryEntryResult(false, "Entry with this name already exists in bestiary");

        try
        {
            var entry = BestiaryEntry.Create(command.MasterId, command.Name, command.MaxHitPoints, command.Description);
            
            await _bestiaryRepository.AddToBestiaryAsync(entry, cancellationToken);
            await _bestiaryRepository.SaveChangesAsync(cancellationToken);
            return new AddBestiaryEntryResult(true, null, entry.BestiaryEntryId);
        }
        catch (Exception e)
        {
            return new AddBestiaryEntryResult(false, e.Message);
        }
    }
}