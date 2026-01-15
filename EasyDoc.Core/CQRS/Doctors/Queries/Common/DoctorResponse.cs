using EasyDoc.Application.Abstractions.Data;
using System.Linq.Expressions;

namespace EasyDoc.Application.CQRS.Doctors.Queries.Common;

public record DoctorResponse(Guid Id,
    string PersonName,
    string PhoneNumber,
    string Department,
    string City,
    string ClinicAddress,
    long DefaultAppointmentTimeInMinutes,
    string? Description,
    string? ProfilePictureUrl);

public static class DoctorResponseMapper
{
    public static Expression<Func<DoctorDetailsReadModel ,DoctorResponse>> ToDoctorResponse = (doctor) => new DoctorResponse(doctor.Id,
            doctor.PersonName,
            doctor.PhoneNumber,
            doctor.Department,
            doctor.City,
            doctor.ClinicAddress,
            doctor.DefaultAppointmentTimeInMinutes,
            doctor.Description,
            doctor.ProfilePictureUrl);
}