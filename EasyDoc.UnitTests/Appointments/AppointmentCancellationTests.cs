using EasyDoc.Domain.Entities.AppointmentAggregate;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class AppointmentCancellationTests
{
    [Fact]
    public void PendingAppointment_CanBeCanceledByPatient()
    {
        var appointment = CreateAppointment();

        Assert.True(appointment.CanBeCanceledByPatient());

        appointment.Cancel();

        Assert.Equal(AppointmentStatus.Canceled, appointment.Status);
    }

    [Fact]
    public void ScheduledAppointment_CanBeCanceledByPatient()
    {
        var appointment = CreateAppointment();

        appointment.Accept();

        Assert.True(appointment.CanBeCanceledByPatient());
    }

    [Fact]
    public void CompletedAppointment_CannotBeCanceledByPatient()
    {
        var appointment = CreateAppointment();

        appointment.SetExamination(new Examination("Diagnosis", "Prescription"));

        Assert.False(appointment.CanBeCanceledByPatient());
    }

    [Fact]
    public void CanceledAppointment_CannotBeCanceledAgain()
    {
        var appointment = CreateAppointment();

        appointment.Cancel();

        Assert.False(appointment.CanBeCanceledByPatient());
    }

    private static Appointment CreateAppointment() =>
        new(Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 11, 23),
            new TimeOnly(9, 0),
            new TimeOnly(9, 30));
}
