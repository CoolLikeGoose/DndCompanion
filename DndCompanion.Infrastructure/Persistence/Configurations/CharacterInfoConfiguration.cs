using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CharacterInfoConfiguration : IEntityTypeConfiguration<CharacterInfo>
{
    public void Configure(EntityTypeBuilder<CharacterInfo> builder)
    {
        builder.ToTable("CharacterInfos");
        builder.HasKey(x => x.CharacterId);
        builder.Property(x => x.CharacterId).ValueGeneratedNever();
    }
}