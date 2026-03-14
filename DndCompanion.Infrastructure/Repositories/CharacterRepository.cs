using DndCompanion.Application.Abstractions.Persistence;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class CharacterRepository : ICharacterRepository
{
    private readonly DndCompanionDbContext _dbContext;
    
    public CharacterRepository(DndCompanionDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(Character character, CancellationToken cancellationToken = default)
    {
        await _dbContext.Characters.AddAsync(character, cancellationToken);
    }

    public Task<List<Character>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Characters
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);   
    }
}