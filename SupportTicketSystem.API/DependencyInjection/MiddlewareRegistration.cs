using SupportTicketSystem.Infrastructure.Identity;

namespace SupportTicketSystem.API.DependencyInjection;

public static class MiddlewareRegistration
{
    public static async Task UseApplicationMiddleware(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        // Seed Identity Data
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider
                .GetRequiredService<IIdentitySeeder>();

            await seeder.SeedAsync();
        }

        app.MapControllers();
    }
}