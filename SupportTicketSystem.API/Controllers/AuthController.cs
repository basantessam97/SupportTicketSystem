using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.Application.Abstractions.Services;
using SupportTicketSystem.Application.DTOs.Auth;
using SupportTicketSystem.Domain.Entities;

namespace SupportTicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(
            request.Email);

        if (user is null || !user.IsActive)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var passwordValid =
            await userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var roles = await userManager.GetRolesAsync(user);

        var tokenResult = await jwtTokenService
            .GenerateTokenAsync(user);

        var response = new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,

            ExpiresAt = tokenResult.ExpiresAt,

            User = new UserResponse
            {
                Id = user.Id,

                FullName = user.FullName,

                Email = user.Email!,

                Role = roles.FirstOrDefault()
                    ?? string.Empty
            }
        };

        return Ok(response);
    }
}