using SupportTicketSystem.Domain.Common;

namespace SupportTicketSystem.Domain.Entities;

public class TimeEntry : BaseEntity
{
    public int TicketId { get; set; }

    public string UserId { get; set; }

    public DateTime WorkDate { get; set; }

    public int DurationMinutes { get; set; }

    public string? Description { get; set; }


    public Ticket Ticket { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}
