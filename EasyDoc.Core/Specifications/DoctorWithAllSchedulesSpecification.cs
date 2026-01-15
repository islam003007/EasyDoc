using Ardalis.Specification;
using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Application.Specifications;

internal class DoctorWithAllSchedulesSpecification : Specification<Doctor>
{
    public DoctorWithAllSchedulesSpecification(Guid doctorId)
    {
        Query
            .Where(d => d.Id == doctorId)
            .Include(d => d.Schedules)
            .Include(d => d.ScheduleOverrides);
    }
}
