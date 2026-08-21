using Domain.Entities;
using Infrastructure.Repositories;

namespace DndCompanion.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task AddAndFindByEmail_RoundTrips()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new UserRepository(context);

        var user = User.Create("TestUser", "test@example.com", "hashed-password");
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        var found = await repository.FindByEmailAsync("test@example.com");

        Assert.NotNull(found);
    }

    [Fact]
    public async Task ExistsByEmail_ReturnsTrue_WhenUserExists()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new UserRepository(context);

        var user = User.Create("TestUser", "test@example.com", "hashed-password");
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        Assert.True(await repository.ExistsByEmailAsync("test@example.com"));
        Assert.False(await repository.ExistsByEmailAsync("nobody@example.com"));
    }

    [Fact]
    public async Task ExistsById_ReturnsFalse_AfterUserDeleted()
    {
        await using var context = TestDbContextFactory.Create();
        var repository = new UserRepository(context);

        var user = User.Create("test@example.com", "hashed-password", "TestUser");
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        context.Users.Remove(user);
        await repository.SaveChangesAsync();

        Assert.False(await repository.ExistsById(user.Id));
    }
}