using SupportTicketSystem.Domain.Entities;
using System.Linq.Expressions;

namespace SupportTicketSystem.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<ApplicationUser>> GetAllAsync(
        Expression<Func<ApplicationUser, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}