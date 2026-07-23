using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        
        builder.Property(x => x.CharacterId).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Group).HasMaxLength(100);

        builder.Property(x => x.CurrentValue).IsRequired();
        builder.Property(x => x.MaxValue).IsRequired();
        builder.Property(x => x.RecoveryType).IsRequired();
        
        builder.HasIndex(x => x.CharacterId);
        builder.HasIndex(x => new {x.CharacterId, x.Type, x.Name}).IsUnique();
    }
}