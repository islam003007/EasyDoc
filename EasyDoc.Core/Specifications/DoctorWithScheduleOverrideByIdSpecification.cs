using Ardalis.Specification;
using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Application.Specifications;

internal class DoctorWithScheduleOverrideByIdSpecification : Specification<Doctor>
{
    public DoctorWithScheduleOverrideByIdSpecification(Guid doctorId, Guid scheduleOverrideId)
    {
        Query
            .Where(d => d.Id == doctorId)
            .Include(d => d.ScheduleOverrides.Where(s => s.Id == scheduleOverrideId));
    }
}
