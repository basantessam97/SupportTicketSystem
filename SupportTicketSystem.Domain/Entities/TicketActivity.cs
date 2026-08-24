using SupportTicketSystem.Domain.Common;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Domain.Entities;

public class TicketActivity : BaseEntity
{
    public int TicketId { get; set; }

    public string UserId { get; set; } = null!;

    public ActivityType ActivityType { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Description { get; set; }

    public int? CommentId { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual Comment? Comment { get; set; }
}
