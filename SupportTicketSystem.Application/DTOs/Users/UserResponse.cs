using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs.Users;

public class UserResponse
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserType UserType { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }
}