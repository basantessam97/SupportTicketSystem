using SupportTicketSystem.Application.Abstractions.Repositories;
using SupportTicketSystem.Application.Abstractions.Services;
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

        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // JWT Settings
        services.Configure<JwtSettings>(
            configuration.GetSection("JwtSettings"));

        // JWT Service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Identity Seeder
        services.AddScoped<IIdentitySeeder, IdentitySeeder>();

        return services;
    }
}