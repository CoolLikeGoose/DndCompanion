using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.RemoveMonster;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Monsters;

public class RemoveMonsterServiceTests
{
    private readonly Mock<ISessionRepository> _sessionRepository = new();
    private readonly RemoveMonsterService _service;

    public RemoveMonsterServiceTests()
    {
        _service = new RemoveMonsterService(_sessionRepository.Object);
    }

    [Fact]
    public async Task Fails_WhenSessionNotFound()
    {
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Session?)null);

        var result = await _service.ExecuteAsync(new RemoveMonsterCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenMonsterNotFound()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(new RemoveMonsterCommand(session.Id, Guid.NewGuid()));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Succeeds_AndRemovesMonster_WhenValid()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        var monster = session.AddMonster("Goblin", 10);
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(new RemoveMonsterCommand(session.Id, monster.Id));

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(session.Monsters, m => m.Id == monster.Id);
        _sessionRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}