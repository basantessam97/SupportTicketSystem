namespace SupportTicketSystem.Application.DTOs.Dashboard;

public class AgentWorkloadResponse
{
    public string AgentId { get; set; } = null!;

    public string AgentName { get; set; } = null!;

    public int AssignedTickets { get; set; }
}