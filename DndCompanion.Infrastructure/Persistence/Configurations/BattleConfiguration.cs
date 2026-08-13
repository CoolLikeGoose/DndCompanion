using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BattleConfiguration : IEntityTypeConfiguration<Battle>
{
    public void Configure(EntityTypeBuilder<Battle> builder)
    {
        builder.ToTable("Battles");

        builder.HasKey(b => b.BattleId);
        builder.Property(b => b.BattleId).ValueGeneratedNever();

        builder.Property(b => b.Name).HasMaxLength(100).IsRequired();

        builder.HasOne<Session>()
            .WithMany(s => s.Battles)
            .HasForeignKey(b => b.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}