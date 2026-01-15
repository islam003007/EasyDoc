using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Queries;

public record SearchDoctorsQuery(string SearchTerm,
    Guid? CityId = null,
    Guid? DepartmentId = null,
    int pageNumber = 1,
    int pageSize = PageConstants.DefaultPageSize) : IQuery<IReadOnlyList<SearchDoctorResponse>>;

internal class SearchDoctorsQueryValidator : AbstractValidator<SearchDoctorsQuery>
{
    public SearchDoctorsQueryValidator()
    {
        RuleFor(x => x.SearchTerm).NotEmpty();

        RuleFor(x => x.pageNumber).GreaterThanOrEqualTo(1);

        RuleFor(x => x.pageSize).InclusiveBetween(1, PageConstants.MaxPageSize);
    }
}

public record SearchDoctorResponse(Guid DoctorId,
    string PersonName,
    string? ProfilePictureURL,
    string City,
    string Department);

//internal class DoctorMapper
//{
//    public static readonly Expression<Func<DoctorSearchReadModel, SearchDoctorResponse>> ToDoctorResponse =
//        (doctor) => new SearchDoctorResponse(doctor.Id, doctor.PersonName, doctor.ProfilePictureURL, doctor.City, doctor.Department);
//}

internal class SearchDoctorsQueryHandler : IQueryHandler<SearchDoctorsQuery, IReadOnlyList<SearchDoctorResponse>>
{
    public Task<Result<IReadOnlyList<SearchDoctorResponse>>> Handle(SearchDoctorsQuery query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}