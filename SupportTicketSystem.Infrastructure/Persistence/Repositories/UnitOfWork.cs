using SupportTicketSystem.Application.Abstractions.Repositories;

namespace SupportTicketSystem.Infrastructure.Persistence.Repositories;

public class UnitOfWork(
    ApplicationDbContext context) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);

        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new GenericRepository<T>(context);

            _repositories[type] = repository;
        }

        return (IRepository<T>)repository;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
    Func<Task<T>> operation,
    CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var result = await operation();

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }
}