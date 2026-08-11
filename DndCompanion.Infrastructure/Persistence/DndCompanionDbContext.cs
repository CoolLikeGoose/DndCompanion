using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DndCompanionDbContext : DbContext
{
    public DndCompanionDbContext(DbContextOptions<DndCompanionDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionParticipant> SessionParticipants => Set<SessionParticipant>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<CharacterInfo> CharacterInfos => Set<CharacterInfo>();
    public DbSet<CharacterStats> CharacterStats => Set<CharacterStats>();
    public DbSet<Monster> Monsters => Set<Monster>();
    public DbSet<BestiaryEntry> BestiaryEntries => Set<BestiaryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DndCompanionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}