using Domain.Entities;

namespace DndCompanion.Application.Abstractions.Persistence;

public interface ISessionRepository
{
    Task AddAsync(Session session, CancellationToken cancellationToken = default);
    Task<Session?> FindByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default);
    Task AddParticipantAsync(SessionParticipant participant, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}