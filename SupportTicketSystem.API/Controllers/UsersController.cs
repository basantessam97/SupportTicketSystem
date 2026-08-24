using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.API.Extensions;
using SupportTicketSystem.Application.Abstractions.Services;
using SupportTicketSystem.Application.DTOs.Users;
using SupportTicketSystem.Application.Features.Tickets;
using SupportTicketSystem.Application.Features.Users;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController(
    IUserService _userService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Add(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var adminId = User.GetUserId();

        if (adminId is null)
            return Unauthorized();

        var result = await _userService.AddAsync(
            request,
            adminId,
            cancellationToken);

        if (result.result == UserActionResult.Unauthorized)
            return Forbid();

        if (result.result == UserActionResult.InvalidUserType)
            return BadRequest("Invalid user type.");

        if (result.result == UserActionResult.EmailAlreadyExists)
            return Conflict("Email already exists.");

        if (result.result == UserActionResult.Failed)
            return BadRequest("Failed to create user.");

        return StatusCode(
            StatusCodes.Status201Created,
            result.data);
    }

    [HttpPut("{id}/edit")]
    public async Task<IActionResult> Update(
        string id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var adminId = User.GetUserId();

        if (adminId is null)
            return Unauthorized();

        var result = await _userService.UpdateAsync(
            id,
            request,
            adminId,
            cancellationToken);

        if (result == UserActionResult.Unauthorized)
            return Forbid();

        if (result == UserActionResult.NotFound)
            return NotFound();

        if (result == UserActionResult.EmailAlreadyExists)
            return Conflict("Email already exists.");

        if (result == UserActionResult.Failed)
            return BadRequest("Failed to update user.");

        return NoContent();
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> ChangePassword(
        string id,
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var adminId = User.GetUserId();

        if (adminId is null)
            return Unauthorized();

        var result = await _userService.ChangePasswordAsync(
            id,
            request,
            adminId,
            cancellationToken);

        if (result == UserActionResult.Unauthorized)
            return Forbid();

        if (result == UserActionResult.NotFound)
            return NotFound();

        if (result == UserActionResult.InvalidPassword)
            return BadRequest("Invalid current password or new password.");

        return NoContent();
    }

    [HttpGet("{id}/get-user")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var adminId = User.GetUserId();

        if (adminId is null)
            return Unauthorized();

        var result = await _userService.GetByIdAsync(
            id,
            adminId,
            cancellationToken);

        if (result.result == UserActionResult.Unauthorized)
            return Forbid();

        if (result.result == UserActionResult.NotFound)
            return NotFound();

        return Ok(result.data);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers(
        CancellationToken cancellationToken)
    {
        var result = _userService.GetCustomers(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("agents")]
    public async Task<IActionResult> GetAgents(
        CancellationToken cancellationToken)
    {
        var result = _userService.GetAgents(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("admins")]
    public async Task<IActionResult> GetAdmins(
        CancellationToken cancellationToken)
    {
        var result = _userService.GetAdmins(
            cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("user-ypes")]
    public IActionResult GetUserTypes()
    {
        return Ok(_userService.GetUserTypes());
    }
}