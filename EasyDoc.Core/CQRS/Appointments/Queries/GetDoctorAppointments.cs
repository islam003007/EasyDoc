using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Appointments.Queries;

public record GetDoctorAppointmentsQuery(int PageNumber = 1, int PageSize = PageConstants.DefaultPageSize)
    : IQuery<IReadOnlyList<AppointmentResponse>>;

internal class GetDoctorAppointmentsQueryValidator : AbstractValidator<GetDoctorAppointmentsQuery>
{
    public GetDoctorAppointmentsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PageConstants.MaxPageSize);
    }
}

internal class GetDoctorAppointmentQueryHandler : IQueryHandler<GetDoctorAppointmentsQuery, IReadOnlyList<AppointmentResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetDoctorAppointmentQueryHandler(IReadOnlyApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> HandleAsync(GetDoctorAppointmentsQuery query, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var appointments = await (from a in _dbContext.Appointments
                                  join p in _dbContext.Patients on a.PatientId equals p.Id
                                  where a.DoctorId == doctorId
                                  orderby a.Date descending, a.StartTime descending
                                  select new AppointmentResponse(
                                      a.Id,
                                      a.PatientId,
                                      a.DoctorId,
                                      a.Date,
                                      a.StartTime,
                                      a.EndTime,
                                      a.Status,
                                      a.Examination,
                                      p.PersonName,
                                      null))
                                  .Skip((query.PageNumber - 1) * query.PageSize)
                                  .Take(query.PageSize)
                                  .ToListAsync(cancellationToken);

        return appointments;
    }
}