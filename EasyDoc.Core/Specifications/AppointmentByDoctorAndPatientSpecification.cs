using Ardalis.Specification;
using EasyDoc.Domain.Entities.AppointmentAggregate;

namespace EasyDoc.Application.Specifications;

internal class AppointmentByDoctorAndPatientSpecification : Specification<Appointment>
{
    public AppointmentByDoctorAndPatientSpecification(Guid doctorId,
        Guid patientId,
        DateOnly date,
        Guid? excludedAppointmentId = null)
    {
        Query
            .Where(a => a.DoctorId == doctorId)
            .Where(a => a.PatientId == patientId)
            .Where(a => a.Date == date)
            // A canceled visit is not an existing booking. Pending, scheduled and completed
            // visits still count for the one-visit-per-doctor-per-day rule.
            .Where(a => a.Status != AppointmentStatus.Canceled);

        if (excludedAppointmentId.HasValue)
        {
            var excludedId = excludedAppointmentId.Value;
            Query.Where(a => a.Id != excludedId);
        }
    }
}
