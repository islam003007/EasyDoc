using Ardalis.GuardClauses;
using EasyDoc.Domain.Constants;
using EasyDoc.Domain.Gaurds;

namespace EasyDoc.Domain.Entities.AppointmentAggregate
{
    public class Appointment : BaseEntity, IAggregateRoot
    {
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public DateOnly Date { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public Examination? Examination { get; private set; }
        public Appointment(Guid patientProfileId, Guid doctorProfileId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            Guard.Against.Default(patientProfileId, nameof(patientProfileId));
            Guard.Against.Default(doctorProfileId, nameof(doctorProfileId));
            Guard.Against.Default(date, nameof(date));
            Guard.Against.Default(startTime, nameof(startTime));
            Guard.Against.Default(endTime, nameof(endTime));
            Guard.Against.StartTimeAfterEndTime(startTime, endTime);
            Guard.Against.OutOfRange(endTime,
                nameof(endTime),
                startTime.AddMinutes(AppointmentConstants.MinAppointmentTimeInMinutes),
                startTime.AddMinutes(AppointmentConstants.MaxAppointmentTimeInMinutes)); // so that an appointment doesn't take up the whole time

            PatientId = patientProfileId;
            DoctorId = doctorProfileId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Status = AppointmentStatus.Pending; 
        }
        private Appointment() { }
        public void Accept()
        { 
            Status = AppointmentStatus.Scheduled; 
        }

        public void Cancel()
        {
            Status = AppointmentStatus.Canceled; 
        }

        public void SetExamination(Examination examination)
        {
            Guard.Against.Null(examination, nameof(examination));
            
            Examination = examination;
            Status = AppointmentStatus.completed;
        }

    }
}
