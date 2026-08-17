using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BestiaryEntryConfiguration : IEntityTypeConfiguration<BestiaryEntry>
{
    public void Configure(EntityTypeBuilder<BestiaryEntry> builder)
    {
        builder.ToTable("BestiaryEntries");
        
        builder.HasKey(x => x.BestiaryEntryId);
        builder.Property(x => x.BestiaryEntryId).ValueGeneratedNever();
        
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.MaxHp)
            .IsRequired();
        
        builder.HasIndex(x => new { x.MasterId, x.Name }).IsUnique();
    }
}