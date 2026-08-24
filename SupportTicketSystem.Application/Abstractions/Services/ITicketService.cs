using SupportTicketSystem.Application.DTOs.Tickets;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Abstractions.Services;

public interface ITicketService
{
    Task<(TicketActionResult result, CustomerTicketResponse? data)> CreateAsync(
    CreateTicketRequest request,
    string customerId,
    CancellationToken cancellationToken = default);

    Task<(TicketActionResult result, TicketDataResponse? data)> GetByIdAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default);

    Task<(TicketActionResult result, IReadOnlyList<TicketCommentResponse>? data)> GetCommentsAsync(
    int ticketId,
    int pageNumber,
    int pageSize,
    string userId,
    CancellationToken cancellationToken = default);

    Task<(TicketActionResult result, IReadOnlyList<TicketActivityResponse>? data)> GetActivitiesAsync(
    int ticketId,
    int pageNumber,
    int pageSize,
    string userId,
    CancellationToken cancellationToken = default);

    Task<(TicketActionResult result, IReadOnlyList<TicketTimeEntryResponse>? data)> GetTimeEntriesAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default);

    Task<TicketActionResult> ChangeActivationAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default);

    Task<TicketActionResult> ChangePriorityAsync(
    int ticketId,
    int priority,
    string userId,
    CancellationToken cancellationToken = default);

    Task<TicketActionResult> ChangeStatusAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default);

    Task<TicketActionResult> AssignTicketAsync(
    int ticketId,
    string agentId,
    string adminId,
    CancellationToken cancellationToken = default);

    IReadOnlyList<LookupResponse> GetPriorities();

    IReadOnlyList<LookupResponse> GetStatuses();

    IReadOnlyList<AgentResponse> GetAgentsAsync();

    Task<(TicketActionResult result, CommentResponse? data)> AddCommentAsync(
    int ticketId,
    AddCommentRequest request,
    string userId,
    CancellationToken cancellationToken = default);

    Task<(TicketActionResult result, TimeEntryResponse? data)> LogTimeAsync(
    int ticketId,
    LogTimeRequest request,
    string userId,
    CancellationToken cancellationToken = default);

    Task<(TicketActionResult result, TicketGridResponse? data)> GetGridAsync(
    TicketGridRequest request,
    string userId,
    CancellationToken cancellationToken = default);
}