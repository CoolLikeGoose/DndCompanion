using DndCompanion.Application.Abstractions.Identity;
using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.UpdateBestiaryEntry;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Bestiary;

public class UpdateBestiaryEntryServiceTests
{
    private readonly Mock<IBestiaryRepository> _repository = new();
    private readonly Mock<ICurrentUser> _currentUser = new();
    private readonly UpdateBestiaryEntryService _service;

    public UpdateBestiaryEntryServiceTests()
    {
        _service = new UpdateBestiaryEntryService(_repository.Object, _currentUser.Object);
    }

    [Fact]
    public async Task Fails_WhenEntryNotFound()
    {
        _repository
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BestiaryEntry?)null);

        var result = await _service.ExecuteAsync(new UpdateBestiaryEntryCommand(Guid.NewGuid(), "New Name", 10, null));

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

        var result = await _service.ExecuteAsync(
            new UpdateBestiaryEntryCommand(entry.BestiaryEntryId, "New Name", 10, null));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Succeeds_WhenOwnerMatches()
    {
        var masterId = Guid.NewGuid();
        var entry = BestiaryEntry.Create(masterId, "Goblin", 10);
        _repository
            .Setup(x => x.FindByIdAsync(entry.BestiaryEntryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _currentUser.Setup(x => x.UserId).Returns(masterId);

        var result = await _service.ExecuteAsync(
            new UpdateBestiaryEntryCommand(entry.BestiaryEntryId, "Updated Goblin", 15, null));

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Goblin", entry.Name);
        Assert.Equal(15, entry.MaxHp);
    }

    [Fact]
    public async Task SavesChanges_WhenSuccessful()
    {
        var masterId = Guid.NewGuid();
        var entry = BestiaryEntry.Create(masterId, "Goblin", 10);
        _repository
            .Setup(x => x.FindByIdAsync(entry.BestiaryEntryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        _currentUser.Setup(x => x.UserId).Returns(masterId);

        await _service.ExecuteAsync(new UpdateBestiaryEntryCommand(entry.BestiaryEntryId, "Updated", 10, null));

        _repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}