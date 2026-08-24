namespace SupportTicketSystem.Application.DTOs.Tickets;

public class TicketGridItemResponse
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string? AssignedAgentName { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ResolvedOn { get; set; }

    public DateTime? ClosedOn { get; set; }
}