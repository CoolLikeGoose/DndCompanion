using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CharacterStatsConfiguration : IEntityTypeConfiguration<CharacterStats>
{
    public void Configure(EntityTypeBuilder<CharacterStats> builder)
    {
        builder.ToTable("CharacterStats");
        builder.HasKey(x => x.CharacterId);
        builder.Property(x => x.CharacterId).ValueGeneratedNever();
    }
}