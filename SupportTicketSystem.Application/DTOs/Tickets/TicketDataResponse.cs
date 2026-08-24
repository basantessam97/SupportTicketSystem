using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs.Tickets;

public class TicketDataResponse
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public string? CustomerName { get; set; } 

    public string? AssignedAgentName { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ResolvedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    public bool IsActive { get; set; }

    public bool IsCustomer { get; set; }

    public bool IsAgent { get; set; }
}