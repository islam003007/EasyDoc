using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using EasyDoc.Application.Specifications;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using Xunit;

namespace EasyDoc.UnitTests.Appointments;

public class AppointmentDuplicateSpecificationTests
{
    private static readonly Guid DoctorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherDoctorId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid PatientId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid OtherPatientId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly DateOnly Day = new(2026, 11, 23);

    private sealed class TestAppointment : Appointment
    {
        public TestAppointment(Guid patientId, Guid doctorId, DateOnly date)
            : base(patientId, doctorId, date, new TimeOnly(9, 0), new TimeOnly(9, 30))
        {
        }

        public void SetId(Guid id) => Id = id;
    }

    private static TestAppointment Create(Guid doctorId, Guid patientId, DateOnly date, Guid? id = null)
    {
        var appointment = new TestAppointment(patientId, doctorId, date);
        if (id.HasValue) appointment.SetId(id.Value);
        return appointment;
    }

    private static List<Appointment> Filter(IEnumerable<Appointment> appointments, Guid? excludedId = null)
    {
        var specification = new AppointmentByDoctorAndPatientSpecification(
            DoctorId,
            PatientId,
            Day,
            excludedId);

        return SpecificationEvaluator.Default
            .GetQuery(appointments.AsQueryable(), specification)
            .ToList();
    }

    [Fact]
    public void PendingAndScheduled_CountAsExistingAppointments()
    {
        var pending = Create(DoctorId, PatientId, Day);
        var scheduled = Create(DoctorId, PatientId, Day);
        scheduled.Accept();

        var result = Filter(new[] { pending, scheduled });

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Completed_StillCountsForTheSameDayRule()
    {
        var completed = Create(DoctorId, PatientId, Day);
        completed.SetExamination(new Examination("Diagnosis", "Prescription"));

        var result = Filter(new[] { completed });

        Assert.Single(result);
    }

    [Fact]
    public void Canceled_DoesNotCount()
    {
        var canceled = Create(DoctorId, PatientId, Day);
        canceled.Cancel();

        Assert.Empty(Filter(new[] { canceled }));
    }

    [Fact]
    public void OtherDoctorPatientOrDate_DoesNotCount()
    {
        var otherDoctor = Create(OtherDoctorId, PatientId, Day);
        var otherPatient = Create(DoctorId, OtherPatientId, Day);
        var otherDate = Create(DoctorId, PatientId, Day.AddDays(1));

        Assert.Empty(Filter(new[] { otherDoctor, otherPatient, otherDate }));
    }

    [Fact]
    public void ExcludedAppointment_DoesNotCountAgainstItself()
    {
        var currentId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var current = Create(DoctorId, PatientId, Day, currentId);

        Assert.Empty(Filter(new[] { current }, currentId));
    }

    [Fact]
    public void AnotherActiveAppointment_StillCountsWhenOneIsExcluded()
    {
        var currentId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var current = Create(DoctorId, PatientId, Day, currentId);
        var other = Create(DoctorId, PatientId, Day);

        var result = Filter(new[] { current, other }, currentId);

        Assert.Single(result);
        Assert.Same(other, result[0]);
    }
}
