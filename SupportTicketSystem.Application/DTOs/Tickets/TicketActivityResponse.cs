namespace SupportTicketSystem.Application.DTOs.Tickets;

public class TicketActivityResponse
{
    public int Id { get; set; }

    public string ActivityType { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Description { get; set; }

    public string UserName { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
}
