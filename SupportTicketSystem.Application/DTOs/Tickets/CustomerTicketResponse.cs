using SupportTicketSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SupportTicketSystem.Application.DTOs.Tickets;

public class CustomerTicketResponse
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TicketStatus Status { get; set; }

    public DateTime CreatedOn { get; set; }
}
