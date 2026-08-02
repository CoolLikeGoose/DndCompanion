using DndCompanion.Application.Abstractions.Persistence;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class SessionRepository : ISessionRepository
{
    private readonly DndCompanionDbContext _dbContext;
    
    public SessionRepository(DndCompanionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
    {
        await _dbContext.Sessions.AddAsync(session, cancellationToken);
    }

    public async Task<Session?> FindByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default)
    {
        var normalized = InviteCode.From(inviteCode);
        
        return await _dbContext.Sessions
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(x => x.InviteCode == normalized, cancellationToken);
    }

    public async Task<Session?> FindByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
    }

    public async Task<Session?> FindByIdWithParticipantsAndCharactersAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Include(x => x.Participants)
            .ThenInclude(x => x.Character)
            .ThenInclude(x => x!.Resources)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
    }

    public Task<SessionParticipant?> FindParticipantByIdWithItemsAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        return _dbContext.SessionParticipants
            .Include(x => x.Character)
            .ThenInclude(x => x!.Items)
            .FirstOrDefaultAsync(x => x.Id == participantId, cancellationToken);
    }

    public Task<SessionParticipant?> FindParticipantByIdWithInfoAndStatsAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        return _dbContext.SessionParticipants
            .Include(x => x.Character)
            .ThenInclude(x => x!.Info)
            .Include(x => x.Character)
            .ThenInclude(x => x!.Stats)
            .FirstOrDefaultAsync(x => x.Id == participantId, cancellationToken);
    }

    public async Task<SessionParticipant?> FindParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SessionParticipants
            .Include(x => x.Character)
            .ThenInclude(x => x!.Resources)
            .FirstOrDefaultAsync(x => x.Id == participantId, cancellationToken);
    }

    // last session determined by joinedAt
    public Task<SessionParticipant?> FindActiveParticipantByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.SessionParticipants
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.JoinedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddParticipantAsync(SessionParticipant participant, CancellationToken cancellationToken = default)
    {
        await _dbContext.SessionParticipants.AddAsync(participant, cancellationToken);
    }

    public async Task RemoveParticipantsByUserIdAsync(Guid userId, Guid? exceptSessionId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SessionParticipants.Where(x => x.UserId == userId);

        if (exceptSessionId.HasValue)
            query = query.Where(x => x.SessionId != exceptSessionId.Value);

        var toRemove = await query.ToListAsync(cancellationToken);
        if (toRemove.Count > 0)
            _dbContext.SessionParticipants.RemoveRange(toRemove);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}