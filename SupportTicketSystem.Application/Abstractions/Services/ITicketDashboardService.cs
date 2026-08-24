using SupportTicketSystem.Application.DTOs.Dashboard;

namespace SupportTicketSystem.Application.Abstractions.Services;

public interface ITicketDashboardService
{
    Task<TicketCountsResponse> GetTicketCountsAsync(
        CancellationToken cancellationToken = default);

    Task<OpenCriticalTicketsResponse> GetOpenCriticalTicketsAsync(
        CancellationToken cancellationToken = default);

    Task<AverageResolutionTimeResponse> GetAverageResolutionTimeAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AgentWorkloadResponse>> GetAgentWorkloadAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TicketStatusChartResponse>> GetStatusChartAsync(
        CancellationToken cancellationToken = default);
}