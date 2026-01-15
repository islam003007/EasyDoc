using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace EasyDoc.Application.Abstractions.Data;

public interface IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);

    public IDbContextTransaction? CurrentTransaction { get; }
}
