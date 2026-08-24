using SupportTicketSystem.Domain.Common;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Domain.Entities;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TicketStatus Status { get; set; }

    public TicketPriority Priority { get; set; }

    public string CustomerId { get; set; } = null!;

    public string? AssignedAgentId { get; set; }

    public DateTime? ResolvedOn { get; set; }

    public DateTime? ClosedOn { get; set; }


    // Navigation Properties


    public virtual ApplicationUser Customer { get; set; } = null!;

    public virtual ApplicationUser? AssignedAgent { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }
        = new List<Comment>();

    public virtual ICollection<TicketActivity> Activities { get; set; }
        = new List<TicketActivity>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; }
        = new List<TimeEntry>();
}
