namespace SupportTicketSystem.Infrastructure.Identity;

public interface IIdentitySeeder
{
    Task SeedAsync();
}