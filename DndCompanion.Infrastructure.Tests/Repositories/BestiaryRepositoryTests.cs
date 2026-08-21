using Domain.Entities;
using Infrastructure.Repositories;

namespace DndCompanion.Infrastructure.Tests.Repositories;

public class BestiaryRepositoryTests
{
    [Fact]
    public async Task AddAndFindById_RoundTrips()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new BestiaryRepository(context);

        var entry = BestiaryEntry.Create(Guid.NewGuid(), "Goblin", 10);
        await repository.AddToBestiaryAsync(entry);
        await repository.SaveChangesAsync();

        var found = await repository.FindByIdAsync(entry.BestiaryEntryId);

        Assert.NotNull(found);
        Assert.Equal("Goblin", found!.Name);
    }

    [Fact]
    public async Task SearchBestiaryEntries_MatchesByPrefixAndRespectsLimit()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new BestiaryRepository(context);
        var masterId = Guid.NewGuid();

        foreach (var name in new[] { "Goblin", "Goblin Boss", "Ghost", "Skeleton" })
        {
            await repository.AddToBestiaryAsync(BestiaryEntry.Create(masterId, name, 10));
        }
        await repository.SaveChangesAsync();

        var results = await repository.SearchBestiaryEntriesAsync(masterId, "gob", limit: 1);

        Assert.Single(results);
        Assert.StartsWith("Goblin", results[0].Name);
    }

    [Fact]
    public async Task GetBestiaryEntries_OnlyReturnsEntriesForGivenMaster()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new BestiaryRepository(context);
        var masterId = Guid.NewGuid();
        var otherMasterId = Guid.NewGuid();

        await repository.AddToBestiaryAsync(BestiaryEntry.Create(masterId, "Goblin", 10));
        await repository.AddToBestiaryAsync(BestiaryEntry.Create(otherMasterId, "Skeleton", 10));
        await repository.SaveChangesAsync();

        var results = await repository.GetBestiaryEntriesAsync(masterId);

        Assert.Single(results);
        Assert.Equal("Goblin", results[0].Name);
    }

    [Fact]
    public async Task Remove_DeletesEntry()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new BestiaryRepository(context);

        var entry = BestiaryEntry.Create(Guid.NewGuid(), "Goblin", 10);
        await repository.AddToBestiaryAsync(entry);
        await repository.SaveChangesAsync();

        repository.Remove(entry);
        await repository.SaveChangesAsync();

        var found = await repository.FindByIdAsync(entry.BestiaryEntryId);
        Assert.Null(found);
    }
}