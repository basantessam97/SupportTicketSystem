using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.Application.Abstractions.Services;

namespace SupportTicketSystem.API.Controllers;

[ApiController]
[Route("api/tickets-dashboard")]
[Authorize(Roles = "Admin")]
public class TicketDashboardController(
    ITicketDashboardService _dashboardService)
    : ControllerBase
{
    [HttpGet("counts")]
    public async Task<IActionResult> GetTicketCounts(
    CancellationToken cancellationToken)
    {
        var result = await _dashboardService
            .GetTicketCountsAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("open-critical")]
    public async Task<IActionResult> GetOpenCriticalTickets(
    CancellationToken cancellationToken)
    {
        var result = await _dashboardService
            .GetOpenCriticalTicketsAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("average-resolution-time")]
    public async Task<IActionResult> GetAverageResolutionTime(
    CancellationToken cancellationToken)
    {
        var result = await _dashboardService
            .GetAverageResolutionTimeAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("agent-workload")]
    public async Task<IActionResult> GetAgentWorkload(
    CancellationToken cancellationToken)
    {
        var result = await _dashboardService
            .GetAgentWorkloadAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("status-chart")]
    public async Task<IActionResult> GetStatusChart(
        CancellationToken cancellationToken)
    {
        var result = await _dashboardService
            .GetStatusChartAsync(cancellationToken);

        return Ok(result);
    }
}