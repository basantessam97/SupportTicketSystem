namespace SupportTicketSystem.Application.DTOs.Tickets;

public class LogTimeRequest
{
    public DateTime WorkDate { get; set; }

    public int DurationMinutes { get; set; }

    public string? Description { get; set; }
}