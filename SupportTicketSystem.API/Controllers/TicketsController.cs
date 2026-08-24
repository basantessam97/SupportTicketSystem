using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.API.Extensions;
using SupportTicketSystem.Application.Abstractions.Services;
using SupportTicketSystem.Application.DTOs.Tickets;
using SupportTicketSystem.Domain.Enums;
using System.Security.Claims;

namespace SupportTicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class TicketsController(ITicketService _ticketService) : ControllerBase
{
    [Authorize]
    [HttpGet("grid")]
    public async Task<IActionResult> GetGrid(
    [FromQuery] TicketGridRequest request,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.GetGridAsync(
            request,
            userId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        return Ok(result.data);
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> Create(
    [FromBody] CreateTicketRequest request,
    CancellationToken cancellationToken)
    {
        var customerId = User.GetUserId();

        if (customerId is null)
            return Unauthorized();

        var result = await _ticketService.CreateAsync(
            request,
            customerId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        return StatusCode(
            StatusCodes.Status201Created,
            result.data);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
    int id,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result.result == TicketActionResult.NotFound)
            return NotFound();

        return Ok(result.data);
    }

    [Authorize]
    [HttpGet("{id:int}/comments")]
    public async Task<IActionResult> GetComments(
    int id,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.GetCommentsAsync(
            id,
            pageNumber,
            pageSize,
            userId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result.result == TicketActionResult.NotFound)
            return NotFound();

        return Ok(result.data);
    }

    [Authorize]
    [HttpGet("{id:int}/activities")]
    public async Task<IActionResult> GetActivities(
    int id,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.GetActivitiesAsync(
            id,
            pageNumber,
            pageSize,
            userId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result.result == TicketActionResult.NotFound)
            return NotFound();

        return Ok(result.data);
    }

    [Authorize(Roles = "Admin,SupportAgent")]
    [HttpGet("{id:int}/time-entries")]
    public async Task<IActionResult> GetTimeEntries(
    int id,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.GetTimeEntriesAsync(
            id,
            userId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result.result == TicketActionResult.NotFound)
            return NotFound();

        return Ok(result.data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/active")]
    public async Task<IActionResult> ChangeActivation(
    int id,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.ChangeActivationAsync(
            id,
            userId,
            cancellationToken);

        if (result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result == TicketActionResult.NotFound)
            return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/priority")]
    public async Task<IActionResult> ChangePriority(
    int id,
    int priority,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.ChangePriorityAsync(
            id,
            priority,
            userId,
            cancellationToken);

        if (result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result == TicketActionResult.NotFound)
            return NotFound();

        if (result == TicketActionResult.InvalidPriority)
            return BadRequest("Invalid priority.");

        return NoContent();
    }

    [Authorize(Roles = "Customer,SupportAgent")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(
    int id,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.ChangeStatusAsync(
            id,
            userId,
            cancellationToken);

        if (result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result == TicketActionResult.NotFound)
            return NotFound();

        if (result == TicketActionResult.InvalidStatus)
            return BadRequest("Invalid status transition.");

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> AssignTicket(
    int id,
    [FromBody] AssignTicketRequest request,
    CancellationToken cancellationToken)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (adminId is null)
            return Unauthorized();

        var result = await _ticketService.AssignTicketAsync(
            id,
            request.AgentId,
            adminId,
            cancellationToken);

        if (result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result == TicketActionResult.NotFound)
            return NotFound();

        if (result == TicketActionResult.InvalidAgent)
            return BadRequest("Support agent was not found.");

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("priorities")]
    public IActionResult GetPriorities()
    {
        return Ok(_ticketService.GetPriorities());
    }

    [Authorize]
    [HttpGet("statuses")]
    public IActionResult GetStatuses()
    {
        return Ok(_ticketService.GetStatuses());
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("agents")]
    public IActionResult GetAgents(
    CancellationToken cancellationToken)
    {
        return Ok(_ticketService.GetAgentsAsync());
    }

    [Authorize]
    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(
    int id,
    [FromBody] AddCommentRequest request,
    CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.AddCommentAsync(
            id,
            request,
            userId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result.result == TicketActionResult.NotFound)
            return NotFound();

        return StatusCode(
            StatusCodes.Status201Created,
            result.data);
    }

    [Authorize(Roles = "SupportAgent")]
    [HttpPost("{id:int}/time-entries")]
    public async Task<IActionResult> LogTime(
    int id,
    [FromBody] LogTimeRequest request,
    CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var result = await _ticketService.LogTimeAsync(
            id,
            request,
            userId,
            cancellationToken);

        if (result.result == TicketActionResult.Unauthorized)
            return Forbid();

        if (result.result == TicketActionResult.NotFound)
            return NotFound();

        return StatusCode(
            StatusCodes.Status201Created,
            result.data);
    }
}