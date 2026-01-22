using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Errors;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.SharedKernel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Application.CQRS.Doctors.Queries;

public record GetDoctorAvailableSlotsQuery(Guid DoctorId, DateOnly Date): IQuery<IReadOnlyList<SlotResponse>>;

public record SlotResponse(TimeOnly StartTime, TimeOnly EndTime);

internal class GetDoctorAvailableSlotsQueryValidator : AbstractValidator<GetDoctorAvailableSlotsQuery>
{
    public GetDoctorAvailableSlotsQueryValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}

internal class GetDoctorAvailableSlotsQueryHandler : IQueryHandler<GetDoctorAvailableSlotsQuery, IReadOnlyList<SlotResponse>>
{
    private readonly IReadOnlyApplicationDbContext _dbcontext;
    public GetDoctorAvailableSlotsQueryHandler(IReadOnlyApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    public async Task<Result<IReadOnlyList<SlotResponse>>> HandleAsync(GetDoctorAvailableSlotsQuery query, CancellationToken cancellationToken = default)
    {
        var doctor = await _dbcontext.Doctors.Where(d => d.Id == query.DoctorId)
            .Include(d => d.Schedules.Where(s => s.DayOfWeek == query.Date.DayOfWeek))
            .Include(d => d.ScheduleOverrides.Where(s => s.Date == query.Date))
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result.Failure<IReadOnlyList<SlotResponse>>(DoctorErrors.NotFound(query.DoctorId));

        var appointments = await _dbcontext.Appointments.Where(a => a.DoctorId == query.DoctorId && a.Date == query.Date)
                                                        .OrderBy(a => a.StartTime)
                                                        .ToListAsync();

        var scheduleOverride = doctor.ScheduleOverrides.FirstOrDefault();
        var schedule = doctor.Schedules.FirstOrDefault();

        TimeOnly shiftStart;
        TimeOnly shiftEnd;

        if (scheduleOverride is not null)
        {
            if (!scheduleOverride.IsAvailable)
                return new();

            shiftStart = scheduleOverride.StartTime!.Value;
            shiftEnd = scheduleOverride.EndTime!.Value;
        }
        else if (schedule is not null)
        {
            shiftStart = schedule.StartTime;
            shiftEnd = schedule.EndTime;
        }
        else
        {
            return new();
        }

        return CalculateAvailableSlots(shiftStart,
            shiftEnd,
            doctor.DefaultAppointmentTimeInMinutes,
            appointments);
    }

    // this method could be part of the domain. it is just safer here without having to assume that the right data is included and sorted.
    private static List<SlotResponse> CalculateAvailableSlots(TimeOnly shiftStart,
        TimeOnly shiftEnd,
        long defaultAppointmentTimeInMinutes,
        IReadOnlyList<Appointment> appointments) // APPOITNMENTS MUST BE ORDERED HERE OR THIS Will FAIL.
    {
        TimeOnly currentStartTime = shiftStart;
        TimeOnly currentEndTime = shiftStart;
        var appointmentIndex = 0;
        List<SlotResponse> timeSlots = new();

        while (currentStartTime.AddMinutes(defaultAppointmentTimeInMinutes) <= shiftEnd)
        {
            currentEndTime = currentStartTime.AddMinutes(defaultAppointmentTimeInMinutes);

            while (appointmentIndex < appointments.Count &&
                appointments[appointmentIndex].EndTime <= currentStartTime)
            {
                appointmentIndex++;
            }

            Appointment? potentialOverlap = appointments.ElementAtOrDefault(appointmentIndex);

            if (potentialOverlap is not null &&
                potentialOverlap.EndTime > currentStartTime &&
                potentialOverlap.StartTime < currentEndTime)
            {
                currentStartTime = potentialOverlap.EndTime;
                continue;
            }

            timeSlots.Add(new (currentStartTime, currentEndTime));
            currentStartTime = currentEndTime;
        }

        return timeSlots;
    }
}