namespace SupportTicketSystem.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class;

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default);
}