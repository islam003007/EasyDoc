using Ardalis.Specification;
using EasyDoc.Domain.Entities.DoctorAggregate;

namespace EasyDoc.Application.Specifications;

internal class DoctorWithScheduleByIdSpecification : Specification<Doctor>
{
    public DoctorWithScheduleByIdSpecification(Guid doctorId, Guid scheduleId)
    {
        Query
            .Where(d => d.Id == doctorId)
            .Include(d => d.Schedules.Where(s => s.Id == scheduleId));
    }
}
