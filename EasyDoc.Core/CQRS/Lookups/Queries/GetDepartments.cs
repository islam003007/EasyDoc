using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Lookups.Queries;

public record LookupResponse(Guid Id, string Name);

public record GetDepartmentsQuery : IQuery<IReadOnlyList<LookupResponse>>;

internal class GetDepartmentsQueryHandler : IQueryHandler<GetDepartmentsQuery, IReadOnlyList<LookupResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetDepartmentsQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<LookupResponse>>> HandleAsync(GetDepartmentsQuery query, CancellationToken cancellationToken = default)
    {
        var departments = await _dbContext.Departments
            .OrderBy(department => department.Name)
            .Select(department => new LookupResponse(department.Id, department.Name))
            .ToListAsync(cancellationToken);

        return departments;
    }
}
