using DndCompanion.Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class BestiaryRepository : IBestiaryRepository
{
    private readonly DndCompanionDbContext _dbContext;

    public BestiaryRepository(DndCompanionDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddToBestiaryAsync(BestiaryEntry bestiaryEntry, CancellationToken cancellationToken = default)
    {
        await _dbContext.BestiaryEntries.AddAsync(bestiaryEntry, cancellationToken);
    }

    public Task<BestiaryEntry?> GetBestiaryEntryAsync(Guid bestiaryEntryId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.BestiaryEntries
            .Where(x => x.BestiaryEntryId == bestiaryEntryId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<BestiaryEntry>> GetBestiaryEntriesAsync(Guid masterId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.BestiaryEntries
            .Where(x => x.MasterId == masterId)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}