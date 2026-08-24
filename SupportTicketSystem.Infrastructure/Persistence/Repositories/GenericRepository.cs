using Microsoft.EntityFrameworkCore;
using SupportTicketSystem.Application.Abstractions.Repositories;
using SupportTicketSystem.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace SupportTicketSystem.Infrastructure.Persistence.Repositories;

public class GenericRepository<T>(
    ApplicationDbContext context) : IRepository<T>
    where T : class
{
    protected readonly ApplicationDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(
            [id],
            cancellationToken);
    }


    public async Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
    Expression<Func<T, bool>>? predicate = null,
    Expression<Func<T, object>>? orderBy = null,
    bool descending = true,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy is not null)
        {
            query = descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(
            predicate,
            cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await DbSet.CountAsync(cancellationToken)
            : await DbSet.CountAsync(
                predicate,
                cancellationToken);
    }

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(
            entity,
            cancellationToken);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        DbSet.Remove(entity);
    }
}