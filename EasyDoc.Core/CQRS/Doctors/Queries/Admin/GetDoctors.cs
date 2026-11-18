using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries.Admin;

public record GetDoctorsQuery(int PageNumber = 1, int PageSize = PageConstants.DefaultPageSize)
    : IQuery<IReadOnlyList<AdminDoctorResponse>>;

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

internal class GetDoctorsQueryHandler : IQueryHandler<GetDoctorsQuery, IReadOnlyList<AdminDoctorResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;

    public GetDoctorsQueryHandler(IReadOnlyApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<AdminDoctorResponse>>> HandleAsync(GetDoctorsQuery query, CancellationToken cancellationToken = default)
    {
        // The admin list intentionally has no IsVisible filter.
        var doctors = await (from doctorDetails in _dbContext.DoctorDetails
                             join user in _dbContext.UserReadModels
                                 on doctorDetails.UserId equals user.UserId
                             orderby doctorDetails.PersonName
                             select new AdminDoctorResponse(
                                 doctorDetails.Id,
                                 doctorDetails.UserId,
                                 doctorDetails.PersonName,
                                 doctorDetails.PhoneNumber,
                                 doctorDetails.Department,
                                 doctorDetails.City,
                                 doctorDetails.ClinicAddress,
                                 doctorDetails.Description,
                                 doctorDetails.ProfilePictureUrl,
                                 doctorDetails.IdCardPictureUrl,
                                 doctorDetails.DefaultAppointmentTimeInMinutes,
                                 doctorDetails.IsVisible,
                                 user.Email))
                             .Skip((query.PageNumber - 1) * query.PageSize)
                             .Take(query.PageSize)
                             .ToListAsync(cancellationToken);

        return doctors;
    }
}
