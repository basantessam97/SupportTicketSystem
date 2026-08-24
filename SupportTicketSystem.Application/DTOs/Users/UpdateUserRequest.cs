using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs.Users;

public class UpdateUserRequest
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }
}