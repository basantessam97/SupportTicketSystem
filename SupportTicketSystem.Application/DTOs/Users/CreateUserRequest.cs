using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.DTOs.Users;

public class CreateUserRequest
{
    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public UserType UserType { get; set; }
}