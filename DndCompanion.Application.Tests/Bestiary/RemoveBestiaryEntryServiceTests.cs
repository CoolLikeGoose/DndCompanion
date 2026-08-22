using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.RemoveBestiaryEntry;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Bestiary;

public class RemoveBestiaryEntryServiceTests
{
    private readonly Mock<IBestiaryRepository> _repository = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly RemoveBestiaryEntryService _service;

    public RemoveBestiaryEntryServiceTests()
    {
        _service = new RemoveBestiaryEntryService(_repository.Object, _currentUser.Object);
    }

    [Fact]
    public async Task Fails_WhenEntryNotFound()
    {
        _repository
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BestiaryEntry?)null);

        var result = await _service.ExecuteAsync(new RemoveBestiaryEntryCommand(Guid.NewGuid()));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenNotOwnedByCurrentUser()
    {
        var entry = BestiaryEntry.Create(Guid.NewGuid(), "Goblin", 10);
        _repository
            .Setup(x => x.FindByIdAsync(entry.BestiaryEntryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _currentUser.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var result = await _service.ExecuteAsync(new RemoveBestiaryEntryCommand(entry.BestiaryEntryId));

        Assert.False(result.IsSuccess);
        _repository.Verify(x => x.Remove(It.IsAny<BestiaryEntry>()), Times.Never);
    }

    [Fact]
    public async Task Succeeds_AndRemoves_WhenOwnerMatches()
    {
        var masterId = Guid.NewGuid();
        var entry = BestiaryEntry.Create(masterId, "Goblin", 10);
        _repository
            .Setup(x => x.FindByIdAsync(entry.BestiaryEntryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _currentUser.Setup(x => x.UserId).Returns(masterId);

        var result = await _service.ExecuteAsync(new RemoveBestiaryEntryCommand(entry.BestiaryEntryId));

        Assert.True(result.IsSuccess);
        _repository.Verify(x => x.Remove(entry), Times.Once);
        _repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}