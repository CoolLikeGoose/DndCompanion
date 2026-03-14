using Domain.Entities;

namespace DndCompanion.Application.Abstractions.Persistence;

public interface ICharacterRepository
{
    Task AddAsync(Character character, CancellationToken cancellationToken = default);
    Task<List<Character>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}