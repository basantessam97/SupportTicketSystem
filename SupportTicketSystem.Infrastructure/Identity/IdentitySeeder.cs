using Microsoft.AspNetCore.Identity;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using SupportTicketSystem.Infrastructure.Persistence;

namespace SupportTicketSystem.Infrastructure.Identity;

public class IdentitySeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext) : IIdentitySeeder
{
    public async Task SeedAsync()
    {
        foreach (var userType in Enum.GetValues<UserType>())
        {
            var roleName = userType.ToString();

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to seed role '{roleName}': {errors}");
                }
            }
        }

        await SeedUserAsync(
            "Admin",
            "admin@supportticket.com",
            "Admin@123",
            UserType.Admin);

        await SeedUserAsync(
            "Support Agent",
            "agent@supportticket.com",
            "Agent@123",
            UserType.SupportAgent);

        await SeedUserAsync(
            "Customer",
            "customer@supportticket.com",
            "Customer@123",
            UserType.Customer);
    }

    private async Task SeedUserAsync(
        string fullName,
        string email,
        string password,
        UserType userType)
    {
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
            return;

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                UserType = userType,
                IsActive = true
            };

            var createResult =
                await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to seed user '{email}': {errors}");
            }

            var roleResult =
                await userManager.AddToRoleAsync(
                    user,
                    userType.ToString());

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to assign role '{userType}' to user '{email}': {errors}");
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                $"Failed to seed user '{email}' with role '{userType}'. " +
                $"Error: {ex.InnerException?.Message ?? ex.Message}",
                ex);
        }
    }
}