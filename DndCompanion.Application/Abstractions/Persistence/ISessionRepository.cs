using Domain.Entities;

namespace DndCompanion.Application.Abstractions.Persistence;

public interface ISessionRepository
{
    Task AddAsync(Session session, CancellationToken cancellationToken = default);
    Task<Session?> FindByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default);
    Task<Session?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Session?> FindByIdWithParticipantsAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Session?> FindByIdWithParticipantsAndCharactersAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Session?> FindByIdWithMonstersAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Monster?> FindMonsterByIdAsync(Guid monsterId, CancellationToken cancellationToken = default);
    Task<SessionParticipant?> FindParticipantByIdWithItemsAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<SessionParticipant?> FindParticipantByIdWithInfoAndStatsAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<SessionParticipant?> FindParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<SessionParticipant?> FindMostRecentParticipantByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddParticipantAsync(SessionParticipant participant, CancellationToken cancellationToken = default);
    Task RemoveParticipantsByUserIdAsync(Guid userId, Guid? exceptSessionId = null, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}