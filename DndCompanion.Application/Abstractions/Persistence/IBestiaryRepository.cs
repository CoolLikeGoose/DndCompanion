using Domain.Entities;

namespace DndCompanion.Application.Abstractions.Persistence;

public interface IBestiaryRepository
{
    Task<BestiaryEntry?> FindByIdAsync(Guid bestiaryEntryId, CancellationToken cancellationToken = default);
    void Remove(BestiaryEntry entry);
    Task AddToBestiaryAsync(BestiaryEntry bestiaryEntry, CancellationToken cancellationToken = default);
    Task<BestiaryEntry?> GetBestiaryEntryAsync(Guid bestiaryEntryId, CancellationToken cancellationToken = default);
    Task<List<BestiaryEntry>> GetBestiaryEntriesAsync(Guid masterId, CancellationToken cancellationToken = default);
    Task<List<BestiaryEntry>> SearchBestiaryEntriesAsync(Guid masterId, string namePrefix, int limit = 5, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}