using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Appointments.Queries.Common;
using EasyDoc.Application.Errors;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Appointments.Queries;

public record GetPatientAppointmentsForDoctorQuery(Guid PatientId) : IQuery<IReadOnlyList<AppointmentResponse>>;

internal class GetPatientAppointmentsForDoctorQueryValidator : AbstractValidator<GetPatientAppointmentsForDoctorQuery>
{
    public GetPatientAppointmentsForDoctorQueryValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty();
    }
}

internal class GetPatientAppointmentsForDoctorQueryHandler : IQueryHandler<GetPatientAppointmentsForDoctorQuery,
    IReadOnlyList<AppointmentResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public GetPatientAppointmentsForDoctorQueryHandler(IReadOnlyApplicationDbContext dbcontext, IUserContext userContext)
    {
        _dbContext = dbcontext;
        _userContext = userContext;
    }

    public async Task<Result<IReadOnlyList<AppointmentResponse>>> Handle(GetPatientAppointmentsForDoctorQuery query,
        CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var allowed = await _dbContext.Appointments.AnyAsync(a => a.DoctorId == doctorId &&
        a.PatientId == query.PatientId &&
        a.Status == AppointmentStatus.Scheduled, cancellationToken);

        if (!allowed)
            return Result.Failure<IReadOnlyList<AppointmentResponse>>(AppointmentErrors.AppointmentRequired);

        var appointments = await _dbContext.Appointments
                                        .Where(a => a.PatientId == query.PatientId && a.Status == AppointmentStatus.completed)
                                        .Select(AppointmentMapper.ToAppointmentResponse)
                                        .ToListAsync(cancellationToken);

        return appointments;
    }
}