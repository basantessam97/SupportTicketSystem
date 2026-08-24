using System.ComponentModel.DataAnnotations;

namespace SupportTicketSystem.Application.DTOs.Tickets;

public class CreateTicketRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;
}