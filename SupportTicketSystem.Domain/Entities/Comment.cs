using SupportTicketSystem.Domain.Common;

namespace SupportTicketSystem.Domain.Entities;

public class Comment : BaseEntity
{
    public int TicketId { get; set; }

    public string UserId { get; set; } = null!;


    public string Content { get; set; } = null!;


    // Navigation Properties

    public Ticket Ticket { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}
