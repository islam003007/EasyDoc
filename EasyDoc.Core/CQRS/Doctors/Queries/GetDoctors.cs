using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries;

public record GetDoctorsQuery(Guid? CityId,
    Guid? DepartmentId,
    int PageNumber = 1,
    int PageSize = PageConstants.DefaultPageSize) : IQuery<IReadOnlyList<GetDoctorsResponse>>;

internal class GetDoctorsQueryValidator : AbstractValidator<GetDoctorsQuery>
{
    public GetDoctorsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PageConstants.MaxPageSize);
    }
}

public record GetDoctorsResponse(Guid Id, string PersonName, string? ProfilePictureUrl);

internal class GetDoctorsQueryHandler : IQueryHandler<GetDoctorsQuery, IReadOnlyList<GetDoctorsResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetDoctorsQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<GetDoctorsResponse>>> Handle(GetDoctorsQuery query, CancellationToken cancellationToken = default)
    {
        var doctorsQuery = _dbContext.Doctors;
                                      

        if (query.CityId is not null)
        {
            doctorsQuery = doctorsQuery.Where(d => d.CityId == query.CityId);
        }

        if (query.DepartmentId is not null)
        {
            doctorsQuery = doctorsQuery.Where(d => d.DepartmentId == query.DepartmentId);
        }

        var doctors = await doctorsQuery.Where(d => d.IsVisible)
                                        .OrderBy(d => d.Id)
                                        .Select((doctor) => new GetDoctorsResponse(doctor.Id, doctor.PersonName, doctor.ProfilePictureUrl))
                                        .ToListAsync(cancellationToken);

        // TODO: Apply is Visible filter. or apply global filter and use IgnoreQueryFilters for admin only.
        return doctors;
    }
}