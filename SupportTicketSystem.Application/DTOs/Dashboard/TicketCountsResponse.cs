namespace SupportTicketSystem.Application.DTOs.Dashboard;

public class TicketCountsResponse
{
    public int Total { get; set; }

    public int Open { get; set; }

    public int InProgress { get; set; }

    public int Resolved { get; set; }

    public int Closed { get; set; }
}