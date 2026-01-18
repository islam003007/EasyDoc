using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Queries;

public record SearchDoctorsQuery(string SearchInput,
    Guid? CityId = null,
    Guid? DepartmentId = null,
    int pageNumber = 1,
    int pageSize = PageConstants.DefaultPageSize) : IQuery<IReadOnlyList<SearchDoctorResponse>>;

internal class SearchDoctorsQueryValidator : AbstractValidator<SearchDoctorsQuery>
{
    public SearchDoctorsQueryValidator()
    {
        RuleFor(x => x.SearchInput)
            .NotEmpty();

        RuleFor(x => x.pageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.pageSize)
            .InclusiveBetween(1, PageConstants.MaxPageSize);
    }
}

public record SearchDoctorResponse(Guid DoctorId,
    string PersonName,
    string? ProfilePictureURL,
    string City,
    string Department); // just to hide the ranking, but requires some extra mapping. The Doctor search read model could be returned as is if desired.

internal class SearchDoctorsQueryHandler : IQueryHandler<SearchDoctorsQuery, IReadOnlyList<SearchDoctorResponse>>
{
    private readonly IDoctorSearchService _searchService;

    public SearchDoctorsQueryHandler(IDoctorSearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<Result<IReadOnlyList<SearchDoctorResponse>>> Handle(SearchDoctorsQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _searchService.SearchDoctorsAsync(query.SearchInput,
            query.CityId,
            query.DepartmentId,
            query.pageNumber,
            query.pageSize,
            cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value
                .Select(s => new SearchDoctorResponse(s.Id, s.PersonName, s.ProfilePictureURL, s.City, s.Department))
                .ToList();
        }
        else
        {
            return Result.Failure<IReadOnlyList<SearchDoctorResponse>>(result.Error);
        }
    }
}