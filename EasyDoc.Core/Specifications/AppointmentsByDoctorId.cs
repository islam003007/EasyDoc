using Ardalis.Specification;
using EasyDoc.Domain.Entities.AppointmentAggregate;

namespace EasyDoc.Application.Specifications;

internal class AppointmentsByDoctorIdSpecification : Specification<Appointment>
{
    public AppointmentsByDoctorIdSpecification(Guid doctorId)
    {
        Query
            .Where(a => a.DoctorId == doctorId);
    }
}
