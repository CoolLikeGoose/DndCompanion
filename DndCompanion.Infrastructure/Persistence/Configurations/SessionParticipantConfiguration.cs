using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SessionParticipantConfiguration : IEntityTypeConfiguration<SessionParticipant>
{
    public void Configure(EntityTypeBuilder<SessionParticipant> builder)
    {
        builder.ToTable("SessionParticipants");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        
        builder.Property(x => x.DisplayName)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(x => x.JoinedAt)
            .IsRequired();
        
        builder.Property(x => x.Role)
            .IsRequired();

        builder.HasIndex(x => x.SessionId);
        
        builder.HasOne(x => x.Character)
            .WithMany()
            .HasForeignKey(x => x.CharacterId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}