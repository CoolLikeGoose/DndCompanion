using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.ReorderMonster;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Monsters;

public class ReorderMonsterServiceTests
{
    private readonly Mock<ISessionRepository> _sessionRepository = new();
    private readonly ReorderMonsterService _service;

    public ReorderMonsterServiceTests()
    {
        _service = new ReorderMonsterService(_sessionRepository.Object);
    }

    [Fact]
    public async Task Fails_WhenSessionNotFound()
    {
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Session?)null);

        var result = await _service.ExecuteAsync(new ReorderMonsterCommand(Guid.NewGuid(), Guid.NewGuid(), 500));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenMonsterNotFound()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(new ReorderMonsterCommand(session.Id, Guid.NewGuid(), 500));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task SetsOrder_WhenValid()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        var monster = session.AddMonster("Goblin", 10);
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(new ReorderMonsterCommand(session.Id, monster.Id, 750));

        Assert.True(result.IsSuccess);
        Assert.Equal(750, monster.Order);
        _sessionRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}