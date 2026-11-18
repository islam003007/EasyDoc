using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Exceptions;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class AppointmentRescheduleTests
{
    [Fact]
    public void PendingAppointment_CanBeRescheduledAndStaysPending()
    {
        var appointment = CreateAppointment();
        var newDate = new DateOnly(2026, 12, 7);
        var newStart = new TimeOnly(11, 0);
        var newEnd = new TimeOnly(11, 30);

        Assert.True(appointment.CanBeRescheduledByPatient());

        appointment.Reschedule(newDate, newStart, newEnd);

        Assert.Equal(newDate, appointment.Date);
        Assert.Equal(newStart, appointment.StartTime);
        Assert.Equal(newEnd, appointment.EndTime);
        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
    }

    [Fact]
    public void ScheduledAppointment_ReturnsToPendingAfterReschedule()
    {
        var appointment = CreateAppointment();
        appointment.Accept();

        appointment.Reschedule(new DateOnly(2026, 12, 8), new TimeOnly(13, 0), new TimeOnly(13, 30));

        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
    }

    [Fact]
    public void CompletedAppointment_CannotBeRescheduledAndIsUnchanged()
    {
        var appointment = CreateAppointment();
        appointment.SetExamination(new Examination("Diagnosis", "Prescription"));
        var date = appointment.Date;
        var startTime = appointment.StartTime;
        var endTime = appointment.EndTime;

        Assert.False(appointment.CanBeRescheduledByPatient());
        Assert.Throws<DomainRuleException>(() =>
            appointment.Reschedule(new DateOnly(2026, 12, 9), new TimeOnly(9, 0), new TimeOnly(9, 30)));

        Assert.Equal(AppointmentStatus.completed, appointment.Status);
        Assert.Equal(date, appointment.Date);
        Assert.Equal(startTime, appointment.StartTime);
        Assert.Equal(endTime, appointment.EndTime);
    }

    [Fact]
    public void CanceledAppointment_CannotBeRescheduledAndIsUnchanged()
    {
        var appointment = CreateAppointment();
        appointment.Cancel();
        var date = appointment.Date;

        Assert.False(appointment.CanBeRescheduledByPatient());
        Assert.Throws<DomainRuleException>(() =>
            appointment.Reschedule(new DateOnly(2026, 12, 10), new TimeOnly(9, 0), new TimeOnly(9, 30)));

        Assert.Equal(AppointmentStatus.Canceled, appointment.Status);
        Assert.Equal(date, appointment.Date);
    }

    [Fact]
    public void InvalidEndTime_CannotLeaveStatusOrTimeHalfUpdated()
    {
        var appointment = CreateAppointment();
        var date = appointment.Date;
        var startTime = appointment.StartTime;
        var endTime = appointment.EndTime;

        Assert.Throws<StartTimeAfterEndTimeException>(() =>
            appointment.Reschedule(new DateOnly(2026, 12, 11), new TimeOnly(10, 0), new TimeOnly(9, 0)));

        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        Assert.Equal(date, appointment.Date);
        Assert.Equal(startTime, appointment.StartTime);
        Assert.Equal(endTime, appointment.EndTime);
    }

    [Fact]
    public void TooShortReschedule_CannotLeaveStatusOrTimeHalfUpdated()
    {
        var appointment = CreateAppointment();
        var date = appointment.Date;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            appointment.Reschedule(new DateOnly(2026, 12, 11), new TimeOnly(10, 0), new TimeOnly(10, 5)));

        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        Assert.Equal(date, appointment.Date);
    }

    private static Appointment CreateAppointment() =>
        new(Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 11, 23),
            new TimeOnly(9, 0),
            new TimeOnly(9, 30));
}
