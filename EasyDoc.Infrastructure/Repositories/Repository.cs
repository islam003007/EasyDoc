using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Infrastructure.Repositories;

internal class Repository<T> : RepositoryBase<T>, IRepository<T>, IRepositoryBase<T>
    where T : class, IAggregateRoot // overridden every method that calls context.SaveChanges to implement Unit Of Work
{                                                                             
    public Repository(DbContext dbContext) : base(dbContext)
    {

    }
    public Repository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
    {

    }

    /// <inheritdoc/>
    public override Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Add(entity);

        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public override Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().AddRange(entities);

        return Task.FromResult(entities);
    }

    /// <inheritdoc/>
    public override Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Update(entity);

        return Task.FromResult(1); // as to one entity was modified
    }

    /// <inheritdoc/>
    public override Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().UpdateRange(entities);

        return Task.FromResult(entities.Count()); // as to the number of entities that were modified
    }

    /// <inheritdoc/>
    public override Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Remove(entity);

        return Task.FromResult(1); // as to one entity was deleted
    }

    /// <inheritdoc/>
    public override Task<int> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().RemoveRange(entities);

        return Task.FromResult(entities.Count());// as to the number of entities that were deleted
    }

    /// <inheritdoc/>
    public override async Task<int> DeleteRangeAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        DbContext.Set<T>().RemoveRange(query);

        return await query.CountAsync(cancellationToken); // query for the number of affected rows before deleting
    }

    /// <inheritdoc/>
    [Obsolete("Direct calls to SaveChangesAsync are not allowed. Use IUnitOfWork.SaveChangesAsync() instead.", true)]
    Task<int> IRepositoryBase<T>.SaveChangesAsync(CancellationToken cancellationToken)
        => throw new InvalidOperationException("Use the IUnitOfWork to commit changes.");
}
