namespace SupportTicketSystem.Application.DTOs.Auth;

public class JwtTokenResult
{
    public string AccessToken { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
}