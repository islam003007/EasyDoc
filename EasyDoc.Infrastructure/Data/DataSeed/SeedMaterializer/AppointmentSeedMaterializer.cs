using EasyDoc.Domain.Entities.AppointmentAggregate;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class AppointmentSeedMaterializer : SeedMaterializerBase<Appointment>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public ExaminationSeedMaterializer? Examination { get; set; }

    public override Appointment ToDomainObject()
    {
        var appointment = new Appointment(PatientId, DoctorId, Date, StartTime, EndTime);

        SetDomainObjectId(appointment);

        if (Examination is not null )
            appointment.SetExamination(Examination.ToDomainObject());

        if (Status == AppointmentStatus.Canceled)
            appointment.Cancel();

        if (Status == AppointmentStatus.Scheduled)
            appointment.Accept();

        return appointment;
    }
}
