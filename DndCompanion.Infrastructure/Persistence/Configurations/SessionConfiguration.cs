using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");
        
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever();
        
        builder.Property(session => session.CreatedAt).IsRequired();
        
        builder.Property(session => session.InviteCode)
            .HasConversion(
                x => x.Value,
                x => InviteCode.From(x))
            .HasMaxLength(10)
            .IsRequired();
        
        builder.HasIndex(session => session.InviteCode).IsUnique();
        
        builder.Property(x => x.PinCode)
            .HasConversion(
                x => x == null ? null : x.Value,
                x => string.IsNullOrWhiteSpace(x) ? null : PinCode.From(x))
            .HasMaxLength(8);
        
        builder.HasMany(session => session.Participants)
            .WithOne()
            .HasForeignKey(participant => participant.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}