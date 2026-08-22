using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.MoveMonsterToBattle;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Monsters;

public class MoveMonsterToBattleServiceTests
{
    private readonly Mock<ISessionRepository> _sessionRepository = new();
    private readonly MoveMonsterToBattleService _service;

    public MoveMonsterToBattleServiceTests()
    {
        _service = new MoveMonsterToBattleService(_sessionRepository.Object);
    }

    [Fact]
    public async Task Fails_WhenSessionNotFound()
    {
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Session?)null);

        var result = await _service.ExecuteAsync(
            new MoveMonsterToBattleCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 500));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenTargetBattleNotFound()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        var monster = session.AddMonster("Goblin", 10);
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(
            new MoveMonsterToBattleCommand(session.Id, monster.Id, Guid.NewGuid(), 500));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task MovesMonster_WhenValid()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        var battle = session.AddBattle("Boss Fight");
        var monster = session.AddMonster("Goblin", 10);
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(
            new MoveMonsterToBattleCommand(session.Id, monster.Id, battle.BattleId, 250));

        Assert.True(result.IsSuccess);
        Assert.Equal(battle.BattleId, monster.BattleId);
        Assert.Equal(250, monster.Order);
        _sessionRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}