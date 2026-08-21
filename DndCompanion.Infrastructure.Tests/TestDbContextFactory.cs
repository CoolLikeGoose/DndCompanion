using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Infrastructure.Tests;

public static class TestDbContextFactory
{
    public static DndCompanionDbContext Create()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DndCompanionDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new DndCompanionDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}