
namespace SupportTicketSystem.Application.DTOs.Tickets;

public class TicketGridRequest
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public int Status { get; set; }

    public int Priority { get; set; }

    public string? AgentId { get; set; }

    public string? SortBy { get; set; } = "CreatedOn";

    public bool SortDescending { get; set; } = true;
}