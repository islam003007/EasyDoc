using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Lookups.Queries;

public record CityLookupResponse(Guid Id, string Name, Guid GovernorateId);

public record GetCitiesQuery : IQuery<IReadOnlyList<CityLookupResponse>>;

internal class GetCitiesQueryHandler : IQueryHandler<GetCitiesQuery, IReadOnlyList<CityLookupResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetCitiesQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<CityLookupResponse>>> HandleAsync(GetCitiesQuery query, CancellationToken cancellationToken = default)
    {
        var cities = await _dbContext.Cities
            .OrderBy(city => city.Name)
            .Select(city => new CityLookupResponse(city.Id, city.Name, city.GovernorateId))
            .ToListAsync(cancellationToken);

        return cities;
    }
}
