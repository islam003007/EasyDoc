using Ardalis.Specification;
using EasyDoc.Domain.Entities.AppointmentAggregate;

namespace EasyDoc.Application.Specifications;

internal class AppointmentByDoctorIdAndDateTimeSpecification : Specification<Appointment>
{
    public AppointmentByDoctorIdAndDateTimeSpecification(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        Query
            .Where(a => a.DoctorId == doctorId)
            .Where(a => a.Date == date)
            .Where(a => a.Status == AppointmentStatus.Pending && a.Status == AppointmentStatus.Scheduled)
            .Where(a => startTime < a.EndTime && endTime > a.StartTime); // covers all overlap cases
    }
}
