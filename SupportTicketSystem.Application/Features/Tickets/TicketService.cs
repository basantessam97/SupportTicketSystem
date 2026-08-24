using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SupportTicketSystem.Application.Abstractions.Repositories;
using SupportTicketSystem.Application.Abstractions.Services;
using SupportTicketSystem.Application.DTOs.Tickets;
using SupportTicketSystem.Domain.Entities;
using SupportTicketSystem.Domain.Enums;
using System.Linq.Expressions;
using System.Net.NetworkInformation;

namespace SupportTicketSystem.Application.Features.Tickets;

public class TicketService(
    IUnitOfWork _unitOfWork,
    UserManager<ApplicationUser> _userManager) : ITicketService
{
    public async Task<(TicketActionResult result, TicketGridResponse? data)> GetGridAsync(
    TicketGridRequest request,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null || !user.IsActive)
            return (TicketActionResult.Unauthorized, null);


        // validate pagination
        if (request.PageNumber < 1)
            request.PageNumber = 1;

        if (request.PageSize < 1)
            request.PageSize = 10;

        if (request.PageSize > 100)
            request.PageSize = 100;


        // search
        string search = string.Empty;

        if (!string.IsNullOrEmpty(request.Search))
            search = request.Search.Trim();


        // filteration
        Expression<Func<Ticket, bool>> predicate = x =>
        (x.IsActive) &&
        (user.UserType == UserType.Admin ||
            (user.UserType == UserType.Customer && x.CustomerId == user.Id) ||
            (user.UserType == UserType.SupportAgent && x.AssignedAgentId == user.Id)) &&
        (string.IsNullOrWhiteSpace(search) ||
            x.TicketNumber.Contains(search) ||
            x.Title.Contains(search) ||
            x.Description.Contains(search)) &&
        (request.Status == 0 ||
            x.Status == (TicketStatus)request.Status) &&
        (request.Priority == 0 ||
            x.Priority == (TicketPriority)request.Priority) &&
        (string.IsNullOrEmpty(request.AgentId) ||
             user.UserType != UserType.Admin ||
             x.AssignedAgentId == request.AgentId);


        // sorting
        Expression<Func<Ticket, object>> orderBy =
            request.SortBy?.ToLower() switch
            {
                "ticketnumber" => x => x.TicketNumber,
                "title" => x => x.Title,
                "status" => x => x.Status,
                "priority" => x => x.Priority,
                "customer" => x => x.Customer.FullName,
                "agent" => x => x.AssignedAgent != null ? x.AssignedAgent.FullName : string.Empty,
                _ => x => x.CreatedOn
            };

        // get data
        var result = await _unitOfWork
            .Repository<Ticket>()
            .GetPagedAsync(
                predicate,
                orderBy,
                request.SortDescending,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        // generate dto list
        var items = result.Items
            .Select(x => new TicketGridItemResponse
            {
                Id = x.Id,
                TicketNumber = x.TicketNumber,
                Title = x.Title,
                Status = x.Status.ToString(),
                Priority = x.Priority.ToString(),
                CustomerName = x.Customer.FullName,
                AssignedAgentName = user.UserType == UserType.Customer ? string.Empty : x.AssignedAgent?.FullName,
                CreatedOn = x.CreatedOn,
                ResolvedOn = x.ResolvedOn,
                ClosedOn = x.ClosedOn
            })
            .ToList();

        return (
            TicketActionResult.Success,
            new TicketGridResponse
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(
                    result.TotalCount / (double)request.PageSize),
                IsAgent = user.UserType == UserType.SupportAgent,
                IsCustomer = user.UserType == UserType.Customer,
                SortingColumns = { "ticketnumber", "title", "status", "priority", "customer", "agent" }
            });
    }

    public async Task<(TicketActionResult result, CustomerTicketResponse? data)> CreateAsync(
    CreateTicketRequest request,
    string customerId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(customerId);

        if (user is null ||
            !user.IsActive ||
            user.UserType != UserType.Customer)
        {
            return (TicketActionResult.Unauthorized, null);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Ticket title is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ArgumentException("Ticket description is required.");

        var ticket = new Ticket
        {
            TicketNumber = GenerateTicketNumber(),

            Title = request.Title.Trim(),

            Description = request.Description.Trim(),

            Status = TicketStatus.Open,

            Priority = TicketPriority.Low,

            CustomerId = customerId,

            AssignedAgentId = null,

            ResolvedOn = null,

            ClosedOn = null,

            CreatedOn = DateTime.UtcNow,

            CreatedBy = customerId,

            IsActive = true
        };

        ticket.Activities.Add(new TicketActivity
        {
            UserId = customerId,

            ActivityType = ActivityType.Created,

            OldValue = null,

            NewValue = ticket.TicketNumber,

            Description = "Ticket created.",

            CreatedOn = DateTime.UtcNow,

            CreatedBy = customerId,

            IsActive = true
        });

        await _unitOfWork.Repository<Ticket>().AddAsync(ticket, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (TicketActionResult.Success, new CustomerTicketResponse
        {
            Id = ticket.Id,

            TicketNumber = ticket.TicketNumber,

            Title = ticket.Title,

            Description = ticket.Description,

            Status = ticket.Status,

            CreatedOn = ticket.CreatedOn
        });
    }

    public async Task<(TicketActionResult result, TicketDataResponse? data)> GetByIdAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId); ;

        if (user is null || !user.IsActive)
            return (TicketActionResult.Unauthorized, null);

        var ticket = await _unitOfWork
            .Repository<Ticket>()
            .GetByIdAsync(ticketId, cancellationToken);

        if (ticket is null)
            return (TicketActionResult.NotFound, null);

        if (!CanAccessTicket(ticket, user))
            return (TicketActionResult.Unauthorized, null);

        return (TicketActionResult.Success, new TicketDataResponse
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            Priority = user.UserType == UserType.Customer
                ? string.Empty
                : ticket.Priority.ToString(),
            CustomerName = ticket.Customer.FullName,
            AssignedAgentName = user.UserType == UserType.Customer
                ? string.Empty
                : ticket.AssignedAgent?.FullName,
            CreatedOn = ticket.CreatedOn,
            ResolvedOn = ticket.ResolvedOn,
            ClosedOn = ticket.ClosedOn,
            IsActive = ticket.IsActive,
            IsAgent = user.UserType == UserType.SupportAgent,
            IsCustomer = user.UserType == UserType.Customer
        });
    }

    public async Task<(TicketActionResult result, CommentResponse? data)> AddCommentAsync(
    int ticketId,
    AddCommentRequest request,
    string userId,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Comment content is required.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null || !user.IsActive)
            return (TicketActionResult.Unauthorized, null);


        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return (TicketActionResult.NotFound, null);

        if (!CanAccessTicket(ticket, user))
            return (TicketActionResult.Unauthorized, null);


        return await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                var comment = new Comment
                {
                    TicketId = ticket.Id,

                    UserId = user.Id,

                    Content = request.Content.Trim(),

                    CreatedOn = DateTime.Now,

                    CreatedBy = user.Id, 
                };

                await _unitOfWork
                    .Repository<Comment>()
                    .AddAsync(comment, cancellationToken);


                ticket.Activities.Add(new TicketActivity
                {
                    TicketId = ticket.Id,

                    UserId = user.Id,

                    ActivityType = ActivityType.CommentAdded,

                    OldValue = null,

                    NewValue = null,

                    Description = request.Content.Trim(),

                    CreatedOn = DateTime.Now,

                    CreatedBy = user.Id,
                });


                await _unitOfWork.SaveChangesAsync(cancellationToken);


                return (
                    TicketActionResult.Success,
                    new CommentResponse
                    {
                        Id = comment.Id,

                        Content = comment.Content,

                        UserName = user.FullName,

                        UserType = user.UserType.ToString(),

                        CreatedOn = comment.CreatedOn
                    });
            },
            cancellationToken);
    }

    public async Task<(TicketActionResult result, TimeEntryResponse? data)> LogTimeAsync(
    int ticketId,
    LogTimeRequest request,
    string userId,
    CancellationToken cancellationToken = default)
    {
        if (request.DurationMinutes <= 0)
            throw new ArgumentException(
                "Duration must be greater than zero.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null ||
            !user.IsActive ||
            user.UserType != UserType.SupportAgent)
        {
            return (TicketActionResult.Unauthorized, null);
        }

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return (TicketActionResult.NotFound, null);

        if (ticket.AssignedAgentId != user.Id)
            return (TicketActionResult.Unauthorized, null);


        return await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                var timeEntry = new TimeEntry
                {
                    TicketId = ticket.Id,

                    UserId = user.Id,

                    WorkDate = request.WorkDate.Date,

                    DurationMinutes = request.DurationMinutes,

                    Description = string.IsNullOrWhiteSpace(request.Description)
                        ? null
                        : request.Description.Trim(),

                    CreatedOn = DateTime.Now,

                    CreatedBy = user.Id,
                };

                await _unitOfWork
                    .Repository<TimeEntry>()
                    .AddAsync(timeEntry, cancellationToken);


                ticket.Activities.Add(new TicketActivity
                {
                    TicketId = ticket.Id,

                    UserId = user.Id,

                    ActivityType = ActivityType.TimeLogged,

                    OldValue = null,

                    NewValue = timeEntry.DurationMinutes.ToString(),

                    Description = $"Time logged =  {request.DurationMinutes} minutes",

                    CreatedOn = DateTime.Now,

                    CreatedBy = user.Id,
                });


                await _unitOfWork.SaveChangesAsync(cancellationToken);


                return (
                    TicketActionResult.Success,
                    new TimeEntryResponse
                    {
                        Id = timeEntry.Id,

                        WorkDate = timeEntry.WorkDate,

                        DurationMinutes = timeEntry.DurationMinutes,

                        Description = timeEntry.Description,

                        UserName = user.FullName
                    });
            },
            cancellationToken);
    }

    public async Task<(TicketActionResult result, IReadOnlyList<TicketCommentResponse>? data)> GetCommentsAsync(
    int ticketId,
    int pageNumber,
    int pageSize,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId); ;

        if (user is null || !user.IsActive)
            return (TicketActionResult.Unauthorized, null);

        // validate pagination
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return (TicketActionResult.NotFound, null);

        if (!CanAccessTicket(ticket, user))
            return (TicketActionResult.Unauthorized, null);

        var comments = ticket.Comments
            .OrderByDescending(x => x.CreatedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TicketCommentResponse
            {
                Id = x.Id,
                Content = x.Content,
                UserName = x.User.FullName,
                UserType = x.User.UserType.ToString(),
                CreatedOn = x.CreatedOn
            })
            .ToList();

        return (TicketActionResult.Success, comments);
    }

    public async Task<(TicketActionResult result, IReadOnlyList<TicketActivityResponse>? data)> GetActivitiesAsync(
    int ticketId,
    int pageNumber,
    int pageSize,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId); ;

        if (user is null || !user.IsActive)
            return (TicketActionResult.Unauthorized, null);

        // validate pagination
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 100)
            pageSize = 100;

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return (TicketActionResult.NotFound, null);

        if (!CanAccessTicket(ticket, user))
            return (TicketActionResult.Unauthorized, null);

        var allowedTypes = new List<ActivityType>();

        if (user.UserType == UserType.Customer)
        {
            allowedTypes.AddRange(
            [
                ActivityType.Created,
                ActivityType.Assigned,
                ActivityType.Resolved,
                ActivityType.Closed
            ]);
        }
        else 
        {
            allowedTypes.AddRange(
            [
                ActivityType.Created,
                ActivityType.Assigned,
                ActivityType.Resolved,
                ActivityType.Closed,
                ActivityType.PriorityChanged,
                ActivityType.ActivityStatusChanged,
                ActivityType.CommentAdded,
                ActivityType.TimeLogged
            ]);
        }

        var activities = ticket.Activities
                .Where(x => allowedTypes.Contains(x.ActivityType))
                .OrderByDescending(x => x.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

        return (TicketActionResult.Success, activities
            .Select(x => new TicketActivityResponse
            {
                Id = x.Id,
                ActivityType = x.ActivityType.ToString(),
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                Description = x.Description,
                UserName = x.User.FullName,
                UserType = x.User.UserType.ToString(),
                CreatedOn = x.CreatedOn
            })
            .ToList());
    }

    public async Task<(TicketActionResult result, IReadOnlyList<TicketTimeEntryResponse>? data)> GetTimeEntriesAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId); ;

        if (user is null || !user.IsActive)
            return (TicketActionResult.Unauthorized, null);

        if (user.UserType == UserType.Customer)
            return (TicketActionResult.Unauthorized, null);

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return (TicketActionResult.NotFound, null);

        if (user.UserType == UserType.SupportAgent &&
            ticket.AssignedAgentId != user.Id)
        {
            return (TicketActionResult.Unauthorized, null);
        }

        var timeEntries = ticket.TimeEntries
            .OrderByDescending(x => x.WorkDate)
            .Select(x => new TicketTimeEntryResponse
            {
                Id = x.Id,
                WorkDate = x.WorkDate,
                DurationMinutes = x.DurationMinutes,
                Description = x.Description,
                UserName = x.User.FullName
            })
            .ToList();

        return (TicketActionResult.Success, timeEntries);
    }

    public async Task<TicketActionResult> ChangeActivationAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId); ;

        if (user is null ||
            !user.IsActive ||
            user.UserType != UserType.Admin)
        {
            return TicketActionResult.Unauthorized;
        }

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null)
            return TicketActionResult.NotFound;


        ticket.IsActive = !ticket.IsActive;
        ticket.UpdatedOn = DateTime.Now;
        ticket.UpdatedBy = user.Id;
        ticket.Activities.Add(new TicketActivity
        {
            UserId = userId,
            ActivityType = ActivityType.ActivityStatusChanged,
            OldValue = ticket.IsActive ? "Disabled" : "Enabled",
            NewValue = ticket.IsActive ? "Enabled" : "Disabled",
            Description = "Ticket has been " + (ticket.IsActive ? "enabled" : "disabled"),
            CreatedOn = DateTime.Now,
            CreatedBy = user.Id,
        });

        _unitOfWork.Repository<Ticket>().Update(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (TicketActionResult.Success);
    }

    public async Task<TicketActionResult> ChangePriorityAsync(
    int ticketId,
    int priority,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId); ;

        if (user is null ||
            !user.IsActive ||
            user.UserType != UserType.Admin)
        {
            return TicketActionResult.Unauthorized;
        }

        if (!Enum.IsDefined(typeof(TicketPriority), priority))
            return TicketActionResult.InvalidPriority;

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return TicketActionResult.NotFound;

        var newPriority = (TicketPriority)priority;

        if (ticket.Priority == newPriority)
            return TicketActionResult.Success;

        var oldPriority = ticket.Priority;

        ticket.Priority = newPriority;
        ticket.UpdatedOn = DateTime.Now;
        ticket.UpdatedBy = user.Id;

        ticket.Activities.Add(new TicketActivity
        {
            UserId = user.Id,
            ActivityType = ActivityType.PriorityChanged,
            OldValue = oldPriority.ToString(),
            NewValue = priority.ToString(),
            Description = "Ticket priority changed.",
            CreatedOn = DateTime.Now,
            CreatedBy = user.Id,
        });

        _unitOfWork.Repository<Ticket>().Update(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TicketActionResult.Success;
    }

    public async Task<TicketActionResult> ChangeStatusAsync(
    int ticketId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId); ;

        if (user is null || !user.IsActive)
            return TicketActionResult.Unauthorized;

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return TicketActionResult.NotFound;

        var oldStatus = ticket.Status;

        if (user.UserType == UserType.SupportAgent)
        {
            if (ticket.AssignedAgentId != user.Id)
                return TicketActionResult.Unauthorized;

            if (ticket.Status != TicketStatus.InProgress)
                return TicketActionResult.InvalidStatus;

            ticket.Status = TicketStatus.Resolved;
            ticket.ResolvedOn = DateTime.Now;
        }
        else if (user.UserType == UserType.Customer)
        {
            if (ticket.CustomerId != user.Id)
                return TicketActionResult.Unauthorized;

            if (ticket.Status != TicketStatus.Resolved)
                return TicketActionResult.InvalidStatus;

            ticket.Status = TicketStatus.Closed;
            ticket.ClosedOn = DateTime.Now;
        }
        else
        {
            return TicketActionResult.Unauthorized;
        }

        ticket.UpdatedOn = DateTime.Now;
        ticket.UpdatedBy = user.Id;

        var activityType =
            ticket.Status == TicketStatus.Resolved
                ? ActivityType.Resolved
                : ActivityType.Closed;

        ticket.Activities.Add(new TicketActivity
        {
            UserId = user.Id,

            ActivityType = activityType,

            OldValue = oldStatus.ToString(),

            NewValue = ticket.Status.ToString(),

            Description = $"Ticket status changed from {oldStatus} to {ticket.Status}.",

            CreatedOn = DateTime.Now,

            CreatedBy = user.Id,

            IsActive = true
        });

        _unitOfWork.Repository<Ticket>().Update(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TicketActionResult.Success;
    }

    public async Task<TicketActionResult> AssignTicketAsync(
    int ticketId,
    string agentId,
    string adminId,
    CancellationToken cancellationToken = default)
    {
        var admin = await _userManager.FindByIdAsync(adminId); 

        if (admin is null ||
            !admin.IsActive ||
            admin.UserType != UserType.Admin)
        {
            return TicketActionResult.Unauthorized;
        }

        var agent = await _userManager.FindByIdAsync(agentId); 

        if (agent is null ||
            !agent.IsActive ||
            agent.UserType != UserType.SupportAgent)
        {
            return TicketActionResult.InvalidAgent;
        }

        var ticket = await GetTicketById(ticketId, _unitOfWork, cancellationToken);

        if (ticket is null || !ticket.IsActive)
            return TicketActionResult.NotFound;


        var oldAgentId = ticket.AssignedAgentId;
        var oldStatus = ticket.Status;

        ticket.AssignedAgentId = agent.Id;
        ticket.Status = TicketStatus.InProgress;

        ticket.UpdatedOn = DateTime.Now;
        ticket.UpdatedBy = admin.Id;

        ticket.Activities.Add(new TicketActivity
        {
            UserId = admin.Id,

            ActivityType = ActivityType.Assigned,

            OldValue = oldAgentId,

            NewValue = agent.Id,

            Description = $"Ticket assigned to {agent.FullName}.",

            CreatedOn = DateTime.Now,

            CreatedBy = admin.Id,

            IsActive = true
        });

        _unitOfWork.Repository<Ticket>().Update(ticket);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TicketActionResult.Success;
    }

    public IReadOnlyList<LookupResponse> GetPriorities()
    {
        return Enum.GetValues<TicketPriority>()
            .Select(x => new LookupResponse
            {
                Id = (int)x,
                Name = x.ToString()
            })
            .ToList();
    }

    public IReadOnlyList<LookupResponse> GetStatuses()
    {
        return Enum.GetValues<TicketStatus>()
            .Select(x => new LookupResponse
            {
                Id = (int)x,
                Name = x.ToString()
            })
            .ToList();
    }

    public IReadOnlyList<AgentResponse> GetAgentsAsync()
    {
        return _userManager.Users
            .Where(x =>
                x.UserType == UserType.SupportAgent &&
                x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new AgentResponse
            {
                Id = x.Id,
                FullName = x.FullName,
            }).ToList();
    }

    private static string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static bool CanAccessTicket(
    Ticket ticket,
    ApplicationUser user)
    {
        return user.UserType switch
        {
            UserType.Admin =>
                true,

            UserType.Customer =>
                ticket.CustomerId == user.Id,

            UserType.SupportAgent =>
                ticket.AssignedAgentId == user.Id,

            _ => false
        };
    }

    private static async Task<Ticket?> GetTicketById(
    int ticketId,
    IUnitOfWork _unitOfWork,
    CancellationToken cancellationToken = default)
    {
        return await _unitOfWork
            .Repository<Ticket>()
            .GetByIdAsync(ticketId, cancellationToken);
    }
}