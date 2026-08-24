namespace SupportTicketSystem.Application.DTOs.Tickets;

public class TicketCommentResponse
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
}
