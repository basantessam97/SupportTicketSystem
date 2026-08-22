using Microsoft.AspNetCore.Identity;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public UserType UserType { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }


    // Navigation Properties

    public ICollection<Ticket> CreatedTickets { get; set; }
        = new List<Ticket>();

    public ICollection<Ticket> AssignedTickets { get; set; }
        = new List<Ticket>();

    public ICollection<Comment> Comments { get; set; }
        = new List<Comment>();

    public ICollection<TicketActivity> Activities { get; set; }
        = new List<TicketActivity>();

    public ICollection<TimeEntry> TimeEntries { get; set; }
        = new List<TimeEntry>();
}
