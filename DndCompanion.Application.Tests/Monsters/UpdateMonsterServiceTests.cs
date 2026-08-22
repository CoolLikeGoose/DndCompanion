using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.UpdateMonster;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Monsters;

public class UpdateMonsterServiceTests
{
    private readonly Mock<ISessionRepository> _sessionRepository = new();
    private readonly UpdateMonsterService _service;

    public UpdateMonsterServiceTests()
    {
        _service = new UpdateMonsterService(_sessionRepository.Object);
    }

    [Fact]
    public async Task Fails_WhenSessionNotFound()
    {
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Session?)null);

        var result = await _service.ExecuteAsync(
            new UpdateMonsterCommand(Guid.NewGuid(), Guid.NewGuid(), "New Name", null, null, null));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenMonsterNotFound()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(
            new UpdateMonsterCommand(session.Id, Guid.NewGuid(), "New Name", null, null, null));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task UpdatesMonster_WhenValid()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        var monster = session.AddMonster("Goblin", 10);
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(
            new UpdateMonsterCommand(session.Id, monster.Id, "Updated Goblin", null, null, null));

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Goblin", monster.Name);
        _sessionRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Fails_AndDoesNotSave_WhenDomainValidationFails()
    {
        var session = Session.Create(Guid.NewGuid(), "Master");
        var monster = session.AddMonster("Goblin", 10);
        _sessionRepository
            .Setup(x => x.FindByIdWithMonstersAsync(session.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _service.ExecuteAsync(
            new UpdateMonsterCommand(session.Id, monster.Id, null, -5, null, null));

        Assert.False(result.IsSuccess);
        _sessionRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}