using Domain.Entities;

namespace DndCompanion.Application.Abstractions.Persistence;

public interface IBestiaryRepository
{
    Task AddToBestiaryAsync(BestiaryEntry bestiaryEntry, CancellationToken cancellationToken = default);
    Task<BestiaryEntry?> GetBestiaryEntryAsync(Guid bestiaryEntryId, CancellationToken cancellationToken = default);
    Task<List<BestiaryEntry>> GetBestiaryEntriesAsync(Guid masterId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}