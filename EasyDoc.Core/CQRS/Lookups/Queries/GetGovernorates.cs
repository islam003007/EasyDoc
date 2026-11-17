using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Lookups.Queries;

public record GovernorateLookupResponse(Guid Id, string Name);

public record GetGovernoratesQuery : IQuery<IReadOnlyList<GovernorateLookupResponse>>;

internal class GetGovernoratesQueryHandler : IQueryHandler<GetGovernoratesQuery, IReadOnlyList<GovernorateLookupResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetGovernoratesQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<GovernorateLookupResponse>>> HandleAsync(GetGovernoratesQuery query, CancellationToken cancellationToken = default)
    {
        var governorates = await _dbContext.Governorates
            .OrderBy(governorate => governorate.Name)
            .Select(governorate => new GovernorateLookupResponse(governorate.Id, governorate.Name))
            .ToListAsync(cancellationToken);

        return governorates;
    }
}
