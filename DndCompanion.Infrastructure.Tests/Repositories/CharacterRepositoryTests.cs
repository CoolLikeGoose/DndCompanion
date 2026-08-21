using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories;

namespace DndCompanion.Infrastructure.Tests.Repositories;

public class CharacterRepositoryTests
{
    [Fact]
    public async Task AddAndFindById_RoundTrips()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new CharacterRepository(context);

        var character = Character.Create("Test Character", Guid.NewGuid());
        await repository.AddAsync(character);
        await repository.SaveChangesAsync();

        var found = await repository.FindByIdAsync(character.Id);

        Assert.NotNull(found);
        Assert.Equal("Test Character", found!.Name);
    }

    [Fact]
    public async Task FindByIdWithResources_LoadsResources()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new CharacterRepository(context);

        var character = Character.Create("Test Character", Guid.NewGuid());
        character.AddResource(ResourceType.HitPoints, 10, RecoveryType.LongRest, "HP");
        await repository.AddAsync(character);
        await repository.SaveChangesAsync();

        var found = await repository.FindByIdWithResourcesAsync(character.Id);

        Assert.Single(found!.Resources);
    }

    [Fact]
    public async Task GetByUserId_OnlyReturnsOwnedCharacters()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new CharacterRepository(context);
        var userId = Guid.NewGuid();

        await repository.AddAsync(Character.Create("Mine", userId));
        await repository.AddAsync(Character.Create("Not mine", Guid.NewGuid()));
        await repository.SaveChangesAsync();

        var results = await repository.GetByUserIdAsync(userId);

        Assert.Single(results);
        Assert.Equal("Mine", results[0].Name);
    }
}