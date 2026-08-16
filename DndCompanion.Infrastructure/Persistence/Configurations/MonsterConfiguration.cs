using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MonsterConfiguration : IEntityTypeConfiguration<Monster>
{
    public void Configure(EntityTypeBuilder<Monster> builder)
    {
        builder.ToTable("Monsters");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.CurrentHp)
            .IsRequired();
        builder.Property(x => x.MaxHp)
            .IsRequired();
        
        builder.HasOne<BestiaryEntry>()
            .WithMany()
            .HasForeignKey(m => m.BestiaryEntryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne<Battle>()
            .WithMany()
            .HasForeignKey(m => m.BattleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(m => m.CreatedAt).IsRequired();
        
        builder.Property(m => m.Order).IsRequired();
        
        builder.HasIndex(x => x.SessionId);
    }
}