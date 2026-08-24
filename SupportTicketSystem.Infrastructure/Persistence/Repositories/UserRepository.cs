using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.Abstractions.Repositories;
using SupportTicketSystem.Domain.Entities;
using System.Linq.Expressions;

namespace SupportTicketSystem.Infrastructure.Persistence.Repositories;

public class UserRepository(
    UserManager<ApplicationUser> userManager) : IUserRepository
{
    public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync(
        Expression<Func<ApplicationUser, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ApplicationUser> query =
            userManager.Users.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }
}