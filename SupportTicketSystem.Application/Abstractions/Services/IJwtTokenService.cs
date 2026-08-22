using SupportTicketSystem.Application.DTOs.Auth;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.Application.Abstractions.Services;

public interface IJwtTokenService
{
    Task<JwtTokenResult> GenerateTokenAsync(
        ApplicationUser user);
}