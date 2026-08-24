using SupportTicketSystem.Application.Abstractions.Repositories;
using SupportTicketSystem.Application.Abstractions.Services;
using SupportTicketSystem.Application.Features.Dashboard;
using SupportTicketSystem.Application.Features.Tickets;
using SupportTicketSystem.Application.Features.Users;
using SupportTicketSystem.Infrastructure.Identity;
using SupportTicketSystem.Infrastructure.Persistence.Repositories;

namespace SupportTicketSystem.API.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Generic Repository
        services.AddScoped(
            typeof(IRepository<>),
            typeof(GenericRepository<>));

        // User Repository
        services.AddScoped<IUserRepository, UserRepository>();

        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // JWT Settings
        services.Configure<JwtSettings>(
            configuration.GetSection("JwtSettings"));

        // JWT Service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Identity Seeder
        services.AddScoped<IIdentitySeeder, IdentitySeeder>();

        // Tickets
        services.AddScoped<ITicketService, TicketService>();

        // Tickets Dashboard
        services.AddScoped<ITicketDashboardService, TicketDashboardService>();

        // Users Management
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}