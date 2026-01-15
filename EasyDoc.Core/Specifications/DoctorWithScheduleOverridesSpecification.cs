using Ardalis.Specification;
using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Application.Specifications;

internal class DoctorWithScheduleOverridesSpecification : Specification<Doctor>
{
    public DoctorWithScheduleOverridesSpecification(Guid doctorId)
    {
        Query
            .Where(d => d.Id == doctorId)
            .Include(d => d.ScheduleOverrides);
    }
}
