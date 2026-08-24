using SupportTicketSystem.Application.DTOs.Tickets;
using SupportTicketSystem.Application.DTOs.Users;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Abstractions.Services;

public interface IUserService
{
    Task<(UserActionResult result, UserResponse? data)> AddAsync(
        CreateUserRequest request,
        string adminId,
        CancellationToken cancellationToken = default);

    Task<UserActionResult> UpdateAsync(
        string userId,
        UpdateUserRequest request,
        string adminId,
        CancellationToken cancellationToken = default);

    Task<UserActionResult> ChangePasswordAsync(
        string userId,
        ChangePasswordRequest request,
        string adminId,
        CancellationToken cancellationToken = default);

    Task<(UserActionResult result, UserResponse? data)> GetByIdAsync(
        string userId,
        string adminId,
        CancellationToken cancellationToken = default);

    IReadOnlyList<UserResponse> GetCustomers(
        CancellationToken cancellationToken = default);

    IReadOnlyList<UserResponse> GetAgents(
        CancellationToken cancellationToken = default);

    IReadOnlyList<UserResponse> GetAdmins(
        CancellationToken cancellationToken = default);

    IReadOnlyList<LookupResponse> GetUserTypes();
}