namespace SupportTicketSystem.Infrastructure.Identity;

public class JwtSettings
{
    public string Key { get; set; } = null!;

    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public int ExpirationInMinutes { get; set; }
}