using Microsoft.AspNetCore.Identity;
using SupportTicketSystem.Application.Abstractions.Repositories;
using SupportTicketSystem.Application.Abstractions.Services;
using SupportTicketSystem.Application.DTOs.Dashboard;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Features.Dashboard;

public class TicketDashboardService(
    IUnitOfWork _unitOfWork,
    IUserRepository _userRepository)
    : ITicketDashboardService
{
    public async Task<TicketCountsResponse> GetTicketCountsAsync(
    CancellationToken cancellationToken = default)
    {
        var tickets = await _unitOfWork
            .Repository<Ticket>()
            .GetAllAsync(
                x => x.IsActive,
                cancellationToken);

        return new TicketCountsResponse
        {
            Total = tickets.Count,

            Open = tickets.Count(x =>
                x.Status == TicketStatus.Open),

            InProgress = tickets.Count(x =>
                x.Status == TicketStatus.InProgress),

            Resolved = tickets.Count(x =>
                x.Status == TicketStatus.Resolved),

            Closed = tickets.Count(x =>
                x.Status == TicketStatus.Closed)
        };
    }

    public async Task<OpenCriticalTicketsResponse> GetOpenCriticalTicketsAsync(
    CancellationToken cancellationToken = default)
    {
        var count = await _unitOfWork
            .Repository<Ticket>()
            .CountAsync(
                x =>
                    x.IsActive &&
                    x.Status != TicketStatus.Closed &&
                    x.Priority == TicketPriority.Critical,
                cancellationToken);

        return new OpenCriticalTicketsResponse
        {
            Count = count
        };
    }

    public async Task<AverageResolutionTimeResponse> GetAverageResolutionTimeAsync(
    CancellationToken cancellationToken = default)
    {
        var timeEntries = await _unitOfWork
            .Repository<TimeEntry>()
            .GetAllAsync(
                x => x.IsActive && (x.Ticket.Status == TicketStatus.Closed || x.Ticket.Status == TicketStatus.Resolved),
                cancellationToken);

        var averageMinutes = timeEntries
            .GroupBy(x => x.TicketId)
            .Select(group => group.Sum(x => x.DurationMinutes))
            .DefaultIfEmpty(0)
            .Average();

        return new AverageResolutionTimeResponse
        {
            AverageHours = Math.Round(
                averageMinutes / 60.0,
                2)
        };
    }

    public async Task<IReadOnlyList<AgentWorkloadResponse>> GetAgentWorkloadAsync(
    CancellationToken cancellationToken = default)
    {
        var agents = await _userRepository.GetAllAsync(
            x =>
                x.UserType == UserType.SupportAgent &&
                x.IsActive,
            cancellationToken);

        var tickets = await _unitOfWork
            .Repository<Ticket>()
            .GetAllAsync(
                x =>
                    x.IsActive &&
                    x.AssignedAgentId != null &&
                    x.Status != TicketStatus.Closed,
                cancellationToken);

        return agents
            .Select(agent => new AgentWorkloadResponse
            {
                AgentId = agent.Id,

                AgentName = agent.FullName,

                AssignedTickets = tickets.Count(x =>
                    x.AssignedAgentId == agent.Id)
            })
            .ToList();
    }

    public async Task<IReadOnlyList<TicketStatusChartResponse>> GetStatusChartAsync(
    CancellationToken cancellationToken = default)
    {
        var tickets = await _unitOfWork
            .Repository<Ticket>()
            .GetAllAsync(
                x => x.IsActive,
                cancellationToken);

        return Enum.GetValues<TicketStatus>()
            .Select(status => new TicketStatusChartResponse
            {
                Status = status.ToString(),

                Count = tickets.Count(x =>
                    x.Status == status)
            })
            .ToList();
    }
}