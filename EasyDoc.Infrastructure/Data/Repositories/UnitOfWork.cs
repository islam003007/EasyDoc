using EasyDoc.Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace EasyDoc.Infrastructure.Data.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IDbContextTransaction? CurrentTransaction => _dbContext.Database.CurrentTransaction;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        => _dbContext.SaveChangesAsync(cancellationToken);
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _dbContext.Database.BeginTransactionAsync(cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        => _dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
}
