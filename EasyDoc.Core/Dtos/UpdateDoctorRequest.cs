using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Dtos;

internal record UpdateDoctorRequest (Guid DoctorId,
    string? PersonName,
    string? PhoneNumber,
    string? ClinicAddress,
    long? DefaultAppointmentTimeInMinutes,
    Optional<string> Description,
    Optional<string> ProfilePictureUrl,
    Guid? CityId = null, // only admin 
    bool? isVisible = null); // only admin
