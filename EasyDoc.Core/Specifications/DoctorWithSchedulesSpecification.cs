using Ardalis.Specification;
using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Application.Specifications;

internal class DoctorWithSchedulesSpecification : Specification<Doctor>
{
    public DoctorWithSchedulesSpecification(Guid doctorId)
    {
        Query
            .Where(doctor => doctor.Id == doctorId)
            .Include(doctor => doctor.Schedules);
    }
}
