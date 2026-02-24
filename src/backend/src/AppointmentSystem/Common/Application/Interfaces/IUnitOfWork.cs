using System.Data;

namespace AppointmentSystem.Common.Application.Interfaces;

/// <summary>
/// Unit of Work abstraction wrapping EF Core SaveChanges with transaction support.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists all pending changes.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>Begins a database transaction.</summary>
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);
    /// <summary>Commits the current transaction.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    /// <summary>Rolls back the current transaction.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
