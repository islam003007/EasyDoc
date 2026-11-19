using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
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

        RuleFor(x => x.Date)
            .NotEmpty()
            .Must(d => d >= ClinicClock.Today).WithMessage("The date cannot be in the past.");

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
        if (query.Date < ClinicClock.Today)
            return new List<SlotResponse>();

        var doctor = await _dbcontext.Doctors.Where(d => d.Id == query.DoctorId)
            .Include(d => d.Schedules.Where(s => s.DayOfWeek == query.Date.DayOfWeek))
            .Include(d => d.ScheduleOverrides.Where(s => s.Date == query.Date))
            .FirstOrDefaultAsync(cancellationToken);

        if (doctor is null)
            return Result.Failure<IReadOnlyList<SlotResponse>>(DoctorErrors.NotFound(query.DoctorId));

        var appointments = await _dbcontext.Appointments.Where(a => a.DoctorId == query.DoctorId && a.Date == query.Date)
                                                        .Where(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Scheduled)
                                                        .OrderBy(a => a.StartTime)
                                                        .ToListAsync();

        var scheduleOverride = doctor.ScheduleOverrides.FirstOrDefault();
        var schedule = doctor.Schedules.FirstOrDefault();

        TimeOnly shiftStart;
        TimeOnly shiftEnd;

        if (scheduleOverride is not null)
        {
            if (!scheduleOverride.IsAvailable)
                return new List<SlotResponse>();

            if (scheduleOverride.StartTime is null || scheduleOverride.EndTime is null)
                return new List<SlotResponse>();

            shiftStart = scheduleOverride.StartTime.Value;
            shiftEnd = scheduleOverride.EndTime.Value;
        }
        else if (schedule is not null)
        {
            shiftStart = schedule.StartTime;
            shiftEnd = schedule.EndTime;
        }
        else
        {
            return new List<SlotResponse>();
        }

        return CalculateAvailableSlots(query.Date,
            shiftStart,
            shiftEnd,
            doctor.DefaultAppointmentTimeInMinutes,
            appointments,
            ClinicClock.Now);
    }

    // Kept internal so slot generation and past-slot filtering can be unit tested without a database.
    internal static List<SlotResponse> CalculateAvailableSlots(DateOnly date,
        TimeOnly shiftStart,
        TimeOnly shiftEnd,
        long defaultAppointmentTimeInMinutes,
        IReadOnlyList<Appointment> appointments, // APPOINTMENTS MUST BE ORDERED BY START TIME.
        DateTime clinicNow)
    {
        if (defaultAppointmentTimeInMinutes <= 0 || shiftEnd <= shiftStart)
            return new List<SlotResponse>();

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

            if (new DateTime(date, currentStartTime) > clinicNow)
                timeSlots.Add(new(currentStartTime, currentEndTime));

            currentStartTime = currentEndTime;
        }

        return timeSlots;
    }
}