using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Infrastructure.Tests.Repositories;

public class SessionRepositoryTests
{
    [Fact]
    public async Task AddAndFindById_RoundTrips()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new SessionRepository(context);

        var session = Session.Create(Guid.NewGuid(), "Master", null);
        await repository.AddAsync(session);
        await repository.SaveChangesAsync();

        var found = await repository.FindByIdAsync(session.Id);

        Assert.NotNull(found);
    }

    [Fact]
    public async Task FindByInviteCode_FindsMatchingSession()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new SessionRepository(context);

        var session = Session.Create(Guid.NewGuid(), "Master", null);
        await repository.AddAsync(session);
        await repository.SaveChangesAsync();

        var found = await repository.FindByInviteCodeAsync(session.InviteCode.Value);

        Assert.NotNull(found);
        Assert.Equal(session.Id, found!.Id);
    }

    [Fact]
    public async Task RemovingBattle_CascadeDeletesMonstersInBattle()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new SessionRepository(context);

        var session = Session.Create(Guid.NewGuid(), "Master", null);
        var battle = session.AddBattle("Boss Fight");
        session.AddMonster("Goblin", 10, battleId: battle.BattleId);

        await repository.AddAsync(session);
        await repository.SaveChangesAsync();

        session.RemoveBattle(battle.BattleId);
        await repository.SaveChangesAsync();

        var monstersLeft = await context.Monsters.CountAsync(m => m.BattleId == battle.BattleId);
        Assert.Equal(0, monstersLeft);
    }

    [Fact]
    public async Task RemovingSession_CascadeDeletesParticipantsAndMonsters()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new SessionRepository(context);

        var session = Session.Create(Guid.NewGuid(), "Master", null);
        session.Join("Player1");
        session.AddMonster("Goblin", 10);

        await repository.AddAsync(session);
        await repository.SaveChangesAsync();

        context.Sessions.Remove(session);
        await repository.SaveChangesAsync();

        Assert.Empty(await context.SessionParticipants.Where(p => p.SessionId == session.Id).ToListAsync());
        Assert.Empty(await context.Monsters.Where(m => m.SessionId == session.Id).ToListAsync());
    }

    [Fact]
    public async Task RemoveParticipantsByUserId_RemovesFromOtherSessions_ButKeepsInExceptedSession()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new SessionRepository(context);
        var userId = Guid.NewGuid();

        var session1 = Session.Create(Guid.NewGuid(), "Master1", null);
        var session2 = Session.Create(Guid.NewGuid(), "Master2", null);
        session1.Join("Player", userId);
        session2.Join("Player", userId);

        await repository.AddAsync(session1);
        await repository.AddAsync(session2);
        await repository.SaveChangesAsync();

        await repository.RemoveParticipantsByUserIdAsync(userId, exceptSessionId: session1.Id);
        await repository.SaveChangesAsync();

        var participantsForUser = await context.SessionParticipants
            .Where(p => p.UserId == userId)
            .ToListAsync();

        Assert.Single(participantsForUser);
        Assert.Equal(session1.Id, participantsForUser[0].SessionId);
    }
}