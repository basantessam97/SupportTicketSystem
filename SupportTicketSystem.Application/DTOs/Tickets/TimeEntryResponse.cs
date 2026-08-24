namespace SupportTicketSystem.Application.DTOs.Tickets;

public class TimeEntryResponse
{
    public int Id { get; set; }

    public DateTime WorkDate { get; set; }

    public int DurationMinutes { get; set; }

    public string? Description { get; set; }

    public string UserName { get; set; } = null!;
}