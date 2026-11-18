using EasyDoc.Application.Services;
using EasyDoc.Domain.Entities;
using EasyDoc.Domain.Entities.DoctorAggregate;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class AppointmentAvailabilityTests
{
    private static readonly DateOnly Monday = new(2026, 11, 23);

    [Fact]
    public void EndTime_UsesTheDoctorsCurrentDefaultDuration()
    {
        var doctor = CreateDoctor(defaultAppointmentTimeInMinutes: 45);

        var resolved = AppointmentService.TryResolveEndTime(doctor, new TimeOnly(9, 15), out var endTime);

        Assert.True(resolved);
        Assert.Equal(new TimeOnly(10, 0), endTime);
    }

    [Fact]
    public void EndTime_ThatCrossesMidnight_IsRejected()
    {
        var doctor = CreateDoctor(defaultAppointmentTimeInMinutes: 45);

        var resolved = AppointmentService.TryResolveEndTime(doctor, new TimeOnly(23, 30), out _);

        Assert.False(resolved);
    }

    [Fact]
    public void IsInTheFuture_RejectsPastAndAcceptsFuture()
    {
        var past = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        Assert.False(AppointmentService.IsInTheFuture(past, new TimeOnly(12, 0)));
        Assert.True(AppointmentService.IsInTheFuture(future, new TimeOnly(12, 0)));
    }

    [Fact]
    public void TimeInsideTheWeeklySchedule_IsAvailable()
    {
        var doctor = CreateDoctor();

        Assert.True(AppointmentService.IsWithinAvailability(doctor,
            Monday,
            new TimeOnly(10, 0),
            new TimeOnly(10, 30)));
    }

    [Fact]
    public void TimeOutsideTheWeeklySchedule_IsNotAvailable()
    {
        var doctor = CreateDoctor();

        Assert.False(AppointmentService.IsWithinAvailability(doctor,
            Monday,
            new TimeOnly(17, 45),
            new TimeOnly(18, 15)));
    }

    [Fact]
    public void DayWithoutASchedule_IsNotAvailable()
    {
        var doctor = CreateDoctor();

        Assert.False(AppointmentService.IsWithinAvailability(doctor,
            new DateOnly(2026, 11, 24),
            new TimeOnly(10, 0),
            new TimeOnly(10, 30)));
    }

    [Fact]
    public void DateOverrideTakesPrecedenceOverTheWeeklySchedule()
    {
        var doctor = CreateDoctor();
        var overrideDate = new DateOnly(2026, 11, 24);
        doctor.AddScheduleOverride(new DoctorScheduleOverride(overrideDate, true, new TimeOnly(12, 0), new TimeOnly(14, 0)));

        Assert.False(AppointmentService.IsWithinAvailability(doctor,
            overrideDate,
            new TimeOnly(10, 0),
            new TimeOnly(10, 30)));

        Assert.True(AppointmentService.IsWithinAvailability(doctor,
            overrideDate,
            new TimeOnly(12, 30),
            new TimeOnly(13, 0)));
    }

    [Fact]
    public void DayOffOverride_IsNotAvailable()
    {
        var doctor = CreateDoctor();
        var dayOff = new DateOnly(2026, 11, 30);
        doctor.AddScheduleOverride(new DoctorScheduleOverride(dayOff, false, null, null));

        Assert.False(AppointmentService.IsWithinAvailability(doctor,
            dayOff,
            new TimeOnly(10, 0),
            new TimeOnly(10, 30)));
    }

    private static Doctor CreateDoctor(long defaultAppointmentTimeInMinutes = 30)
    {
        var doctor = new Doctor(Guid.NewGuid(),
            "Dr. Test",
            new PhoneNumber("+201000000000"),
            "id-card-url",
            "1 Test Clinic St",
            Guid.NewGuid(),
            Guid.NewGuid(),
            defaultAppointmentTimeInMinutes);

        doctor.AddSchedule(new DoctorSchedule(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)));

        return doctor;
    }
}
