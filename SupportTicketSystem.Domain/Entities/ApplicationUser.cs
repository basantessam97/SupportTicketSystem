using Microsoft.AspNetCore.Identity;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public UserType UserType { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }


    // Navigation Properties

    public virtual ICollection<Ticket> CreatedTickets { get; set; }
        = new List<Ticket>();

    public virtual ICollection<Ticket> AssignedTickets { get; set; }
        = new List<Ticket>();

    public virtual ICollection<Comment> Comments { get; set; }
        = new List<Comment>();

    public virtual ICollection<TicketActivity> Activities { get; set; }
        = new List<TicketActivity>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; }
        = new List<TimeEntry>();
}
