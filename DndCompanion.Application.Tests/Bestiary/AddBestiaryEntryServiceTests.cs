using DndCompanion.Application.Abstractions.Persistence;
using DndCompanion.Application.Features.Monsters.AddBestiaryEntry;
using Domain.Entities;
using Moq;

namespace DndCompanion.Application.Tests.Bestiary;

public class AddBestiaryEntryServiceTests
{
    private readonly Mock<IBestiaryRepository> _repository = new();
    private readonly AddBestiaryEntryService _service;

    public AddBestiaryEntryServiceTests()
    {
        _service = new AddBestiaryEntryService(_repository.Object);
        _repository
            .Setup(x => x.GetBestiaryEntriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
    }

    [Fact]
    public async Task Fails_WhenNameAlreadyExists()
    {
        var masterId = Guid.NewGuid();
        var existing = BestiaryEntry.Create(masterId, "Goblin", 10);
        _repository
            .Setup(x => x.GetBestiaryEntriesAsync(masterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([existing]);

        var result = await _service.ExecuteAsync(new AddBestiaryEntryCommand(masterId, "Goblin", 10, null));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Fails_WhenNameExists_IgnoringCaseAndWhitespace()
    {
        var masterId = Guid.NewGuid();
        var existing = BestiaryEntry.Create(masterId, "Goblin", 10);
        _repository
            .Setup(x => x.GetBestiaryEntriesAsync(masterId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([existing]);

        var result = await _service.ExecuteAsync(new AddBestiaryEntryCommand(masterId, "  GOBLIN  ", 10, null));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Succeeds_WhenNameUnique()
    {
        var result = await _service.ExecuteAsync(
            new AddBestiaryEntryCommand(Guid.NewGuid(), "Goblin", 10, null));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.BestiaryEntryId);
    }

    [Fact]
    public async Task PersistsEntry_WhenValid()
    {
        await _service.ExecuteAsync(new AddBestiaryEntryCommand(Guid.NewGuid(), "Goblin", 10, null));

        _repository.Verify(x => x.AddToBestiaryAsync(It.IsAny<BestiaryEntry>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}