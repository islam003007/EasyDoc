using EasyDoc.Domain.Entities.AppointmentAggregate;
using System.Linq.Expressions;

namespace EasyDoc.Application.CQRS.Appointments.Queries.Common;

public record AppointmentResponse(Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentStatus Status,
    Examination? Examination);

public static class AppointmentMapper
{
    public static Expression<Func<Appointment, AppointmentResponse>> ToAppointmentResponse =
        (appointment) => new AppointmentResponse(appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            appointment.Date,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Status,
            appointment.Examination);
}