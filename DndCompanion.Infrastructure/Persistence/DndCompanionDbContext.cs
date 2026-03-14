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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DndCompanionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}