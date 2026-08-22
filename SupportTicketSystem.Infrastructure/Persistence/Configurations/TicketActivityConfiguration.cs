using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Infrastructure.Persistence.Configurations;

public class TicketActivityConfiguration
    : IEntityTypeConfiguration<TicketActivity>
{
    public void Configure(
        EntityTypeBuilder<TicketActivity> builder)
    {
        builder.ToTable("TicketActivities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActivityType)
            .IsRequired();

        builder.Property(x => x.OldValue)
            .HasMaxLength(100);

        builder.Property(x => x.NewValue)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(450);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(450);

        // Ticket
        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Activities)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // User
        builder.HasOne(x => x.User)
            .WithMany(x => x.Activities)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional Comment
        builder.HasOne(x => x.Comment)
            .WithMany()
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.CommentId);

        builder.HasIndex(x => x.CreatedOn);
    }
}
