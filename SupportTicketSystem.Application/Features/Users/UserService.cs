using Microsoft.AspNetCore.Identity;
using SupportTicketSystem.Application.Abstractions.Repositories;
using SupportTicketSystem.Application.Abstractions.Services;
using SupportTicketSystem.Application.DTOs.Tickets;
using SupportTicketSystem.Application.DTOs.Users;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;

namespace SupportTicketSystem.Application.Features.Users;

public class UserService(
    UserManager<ApplicationUser> _userManager,
    IUnitOfWork _unitOfWork)
    : IUserService
{
    public async Task<(UserActionResult result, UserResponse? data)> AddAsync(
        CreateUserRequest request,
        string adminId,
        CancellationToken cancellationToken = default)
    {
        var admin = await _userManager.FindByIdAsync(adminId);

        if (admin is null ||
            !admin.IsActive ||
            admin.UserType != UserType.Admin)
        {
            return (UserActionResult.Unauthorized, null);
        }

        if (!Enum.IsDefined(request.UserType))
            return (UserActionResult.InvalidUserType, null);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
            return (UserActionResult.EmailAlreadyExists, null);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName.Trim(),
            UserType = request.UserType,
            CreatedOn = DateTime.UtcNow,
        };

        var success = await _unitOfWork.ExecuteInTransactionAllContextAsync(async () =>
        {
            var userCreated = await _userManager.CreateAsync(
            user,
            request.Password);

            if (!userCreated.Succeeded)
                return false;


            var roleResult = await _userManager.AddToRoleAsync(
            user,
            request.UserType.ToString());
            if (!roleResult.Succeeded)
                return false;

            return true;
        });

        if(!success)
            return (UserActionResult.Failed, null);

        return (
            UserActionResult.Success,
            MapToResponse(user));
    }

    public async Task<UserActionResult> UpdateAsync(
        string userId,
        UpdateUserRequest request,
        string adminId,
        CancellationToken cancellationToken = default)
    {
        var admin = await _userManager.FindByIdAsync(adminId);

        if (admin is null ||
            !admin.IsActive ||
            admin.UserType != UserType.Admin)
        {
            return UserActionResult.Unauthorized;
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return UserActionResult.NotFound;

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null &&
            existingUser.Id != user.Id)
        {
            return UserActionResult.EmailAlreadyExists;
        }

        user.FullName = request.FullName.Trim();
        user.Email = request.Email;
        user.UserName = request.Email;
        user.IsActive = request.IsActive;
        user.UpdatedOn = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return UserActionResult.Failed;

        return UserActionResult.Success;
    }

    public async Task<UserActionResult> ChangePasswordAsync(
        string userId,
        ChangePasswordRequest request,
        string adminId,
        CancellationToken cancellationToken = default)
    {
        var admin = await _userManager.FindByIdAsync(adminId);

        if (admin is null ||
            !admin.IsActive ||
            admin.UserType != UserType.Admin)
        {
            return UserActionResult.Unauthorized;
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return UserActionResult.NotFound;

        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
            return UserActionResult.InvalidPassword;

        user.UpdatedOn = DateTime.Now;

        await _userManager.UpdateAsync(user);

        return UserActionResult.Success;
    }

    public async Task<(UserActionResult result, UserResponse? data)> GetByIdAsync(
        string userId,
        string adminId,
        CancellationToken cancellationToken = default)
    {
        var admin = await _userManager.FindByIdAsync(adminId);

        if (admin is null ||
            !admin.IsActive ||
            admin.UserType != UserType.Admin)
        {
            return (UserActionResult.Unauthorized, null);
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return (UserActionResult.NotFound, null);

        return (
            UserActionResult.Success,
            MapToResponse(user));
    }

    public IReadOnlyList<UserResponse> GetCustomers(
        CancellationToken cancellationToken = default)
    {
        var users = _userManager.Users
            .Where(x =>
                x.UserType == UserType.Customer &&
                x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email!,
                UserType = x.UserType,
                IsActive = x.IsActive,
                CreatedOn = x.CreatedOn
            })
            .ToList();

        return users;
    }

    public IReadOnlyList<UserResponse> GetAgents(
        CancellationToken cancellationToken = default)
    {
        var users = _userManager.Users
            .Where(x =>
                x.UserType == UserType.SupportAgent &&
                x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email!,
                UserType = x.UserType,
                IsActive = x.IsActive,
                CreatedOn = x.CreatedOn
            })
            .ToList();

        return users;
    }

    public IReadOnlyList<UserResponse> GetAdmins(
        CancellationToken cancellationToken = default)
    {
        var users =  _userManager.Users
            .Where(x =>
                x.UserType == UserType.Admin &&
                x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email!,
                UserType = x.UserType,
                IsActive = x.IsActive,
                CreatedOn = x.CreatedOn
            })
            .ToList();

        return users;
    }

    public IReadOnlyList<LookupResponse> GetUserTypes()
    {
        return Enum.GetValues<UserType>()
            .Select(x => new LookupResponse
            {
                Id = (int)x,
                Name = x.ToString()
            })
            .ToList();
    }

    private static UserResponse MapToResponse(
        ApplicationUser user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            UserType = user.UserType,
            IsActive = user.IsActive,
            CreatedOn = user.CreatedOn
        };
    }
}