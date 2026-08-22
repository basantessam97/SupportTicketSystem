namespace SupportTicketSystem.Application.DTOs.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public UserResponse User { get; set; } = null!;
}

public class UserResponse
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;
}