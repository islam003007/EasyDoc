using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Constants;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Appointments.Queries;

public record GetPatientAppointmentsQuery(int PageNumber = 1, int PageSize = PageConstants.DefaultPageSize)
    : IQuery<IReadOnlyList<AppointmentResponse>>;

internal class GetPatientAppointmentsQueryValidator : AbstractValidator<GetPatientAppointmentsQuery>
{
    public GetPatientAppointmentsQueryValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PageConstants.MaxPageSize);
    }
}

internal class GetPatientAppointmentsQueryHandler : IQueryHandler<GetPatientAppointmentsQuery, IReadOnlyList<AppointmentResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetPatientAppointmentsQueryHandler(IReadOnlyApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> HandleAsync(GetPatientAppointmentsQuery query, CancellationToken cancellationToken = default)
    {
        var patientId = _userContext.PatientId;

        var appointments = await _dbContext.Appointments.Where(a => a.PatientId == patientId)
                                                        .Skip((query.PageNumber - 1) * query.PageSize)
                                                        .Take(query.PageSize)
                                                        .Select(AppointmentMapper.ToAppointmentResponse)
                                                        .ToListAsync(cancellationToken);

        return appointments;
    }
}
