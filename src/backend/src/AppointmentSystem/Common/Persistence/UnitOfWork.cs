using System.Data;
using AppointmentSystem.Common.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppointmentSystem.Common.Persistence;

/// <summary>
/// Implements the Unit of Work pattern wrapping EF Core with transaction management.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    /// <summary>Initializes a new instance of <see cref="UnitOfWork"/>.</summary>
    public UnitOfWork(AppDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public async Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        => _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) throw new InvalidOperationException("No active transaction to commit.");
        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null) return;
        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
}
