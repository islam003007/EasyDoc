using Ardalis.Specification;
using EasyDoc.Domain.Entities.AppointmentAggregate;

namespace EasyDoc.Application.Specifications;

internal class AppointmentByDoctorAndPatientSpecification : Specification<Appointment>
{
    public AppointmentByDoctorAndPatientSpecification(Guid doctorId, Guid patientId, DateOnly date)
    {
        Query
            .Where(a => a.DoctorId == doctorId)
            .Where(a => a.PatientId == patientId)
            .Where(a => a.Date == date);
    }
}
