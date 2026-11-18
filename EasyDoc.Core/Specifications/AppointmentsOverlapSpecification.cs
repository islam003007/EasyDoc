using Ardalis.Specification;
using EasyDoc.Domain.Entities.AppointmentAggregate;

namespace EasyDoc.Application.Specifications;

internal class AppointmentsOverlapSpecification : Specification<Appointment>
{
    public AppointmentsOverlapSpecification(Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludedAppointmentId = null)
    {
        Query
            .Where(a => a.DoctorId == doctorId)
            .Where(a => a.Date == date)
            .Where(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Scheduled)
            .Where(a => startTime < a.EndTime && endTime > a.StartTime); // covers all overlap cases

        if (excludedAppointmentId.HasValue)
        {
            var excludedId = excludedAppointmentId.Value;
            Query.Where(a => a.Id != excludedId);
        }
    }
}
