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

    public async Task AddParticipantAsync(SessionParticipant participant, CancellationToken cancellationToken = default)
    {
        await _dbContext.SessionParticipants.AddAsync(participant, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}